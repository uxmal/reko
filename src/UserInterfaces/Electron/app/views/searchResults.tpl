<div class="table-responsive search-header {{header.cssclass}}">
	<table>
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