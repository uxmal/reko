#!/usr/bin/env php
<?php
$sln = dirname(__FILE__) . "/src/Reko-decompiler.sln";

class SlnParser {
	private $sln;
	private $data;

	public function __construct($sln){
		libxml_use_internal_errors(true);
		
		$this->sln = $sln;
		$this->data = file_get_contents($sln);
	}

	public function __destruct(){
		file_put_contents($this->sln, $this->data);
	}

	const TYPE_GUIDS = array(
		"vcxproj" => "8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942",
		"cproj" => "2857B73E-F847-4B02-9238-064979017E93"
	);

	public static function BuildMap($map){
		return sprintf('{%s}.%s = %s', $map["guid"], $map["sln_conf"], $map["proj_conf"]);
	}

	public static function BuildProject($proj){
		return sprintf('Project("{%s}") = "%s", "%s", "{%s}"',
			$proj["type_guid"],
			$proj["name"],
			$proj["path"],
			$proj["guid"]
		) . PHP_EOL . "EndProject";
	}

	public function EachProject($cb){
		$regex = "/Project\(\"{(.*?)}\"\)\s*=\s*\"(.*?)\",\s*\"(.*?)\",.\s*\"{(.*?)}\"\s*EndProject/";		
		$this->data = preg_replace_callback($regex, function($m) use($cb){
			$typeGUID = $m[1];
			$projName = $m[2];
			$projPath = $m[3];
			$projGUID = $m[4];
		
			$pathExt = strtolower(pathinfo($projPath, PATHINFO_EXTENSION));

			return $cb($m, array(
				"type_guid" => $typeGUID,
				"type" => $pathExt,
				"name" => $projName,
				"path" => $projPath,
				"guid" => $projGUID,
			));
		}, $this->data);
	}

	public function EachMapping($cb){
		$regex = "/{(.*?)}\.(.*?)=(.*)/";
		$this->data = preg_replace_callback($regex, function($m) use($cb){

			$projGUID = $m[1];
			$projConf = rtrim($m[2]);
			$slnConf = ltrim($m[3]);


			return $cb($m, array(
				"guid" => $projGUID,
				"sln_conf" => $projConf,
				"proj_conf" => $slnConf
			));
		}, $this->data);
	}

	private static function fixCsProj($proj){
		$dom = new DomDocument();
		$dom->loadXML(file_get_contents($proj));
		
		{
			$xp = new DOMXPath($dom);
			$all = $xp->query("//*");
			foreach($all as $node){
				foreach($node->attributes as $name => $valNode){
					$valNode->value = rawurldecode($valNode->value);
				}
			}
		}

		$data = $dom->saveXML();
		// libxml will output tags like this		-> <tag/>
		// we prefer a space before the closing tag	-> <tag />
		$data = preg_replace("/<(.*)(?!\s+)\/>/", "<$1 />", $data);

		file_put_contents($proj, $data);
	}

	public function FixEscapes(){
		$this->EachProject(function($m, $proj) {
			if($proj["type"] != "csproj")
				return $m[0];

			$path = dirname($this->sln) . "/" . $proj["path"];
			$path = str_replace('\\', '/', $path);
			self::fixCsProj($path);

			return $m[0];
		});
	}
}

// Placeholder for captured data
$local = array(
	"targetGUID" => null
);

$p = new SlnParser($sln);
$p->EachProject(function($m, $proj) use(&$local) {
	if(
		$proj["name"] == "ArmNative" &&
		($proj["type"] == "vcxproj" || $proj["type"] == "cproj")
	){
		$local["targetGUID"] = $proj["guid"];
		$pi = pathinfo($proj["path"]);
		
		$path = ($pi["dirname"] == "." ? "" : $pi["dirname"]);
		$path.= $pi["filename"] . ".cproj";
		
		$proj["type_guid"] = SlnParser::TYPE_GUIDS["cproj"];
		$proj["path"] = $path;

		return SlnParser::BuildProject($proj);
	}

	return $m[0];
});

if(is_null($local["targetGUID"])){
	throw new Exception("Project not found");
}

$p->EachMapping(function($m, $map) use(&$local){
	if($map["guid"] == $local["targetGUID"]){
		$map["proj_conf"] = strstr($map["proj_conf"], "|", true);
		return SlnParser::BuildMap($map);
	}
	return $m[0];
});

$p->FixEscapes();
?>