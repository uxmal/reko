<tr class=
{{#switch status}}
	{{#case "info"}}"info"{{/case}}
	{{#case "warning"}}"warning"{{/case}}
	{{#case "error"}}"danger"{{/case}}
{{/switch}}>
	<td class="address">{{location}}</td>
	<td>{{message}}</td>
</tr>