{{#each programs}}
<div class="program treenode">
	<span class="program-name">{{this.name}}</span>
	{{#each procedures}}
	<div class="procedure treenode">
		<a href='javascript:void(0)' data-address="{{this.address}}">{{this.name}}</a>
	</div>
	{{/each}}
</div>
{{/each}}