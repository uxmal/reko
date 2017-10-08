<div class="search-header {{header.cssclass}}">
	<table class="table table-striped table-responsive">
		{{#each header.columns}}
		<th class="{{this.cssclass}}">{{this.title}}</th>
		{{/each}}

		{{#each data as |row|}}
			<tr>
			{{#each row as |cell|}}
				<td>{{cell}}</td>
			{{/each}}
			</tr>
		{{/each}}
	</table>
</div>