<template>
	<div>
		<el-button @click="visible = true">Button</el-button>
		<el-dialog v-model="visible" title="Hello world">
			<p>Try Element</p>
		</el-dialog>
	</div>
</template>

<script lang="ts">
	import Vue from 'vue';
	import Component from 'vue-class-component';

	@Component({})
	export default class Test extends Vue {
		visible:boolean = false;
		data = function() {
			return { visible: false }
		}
	}
</script>

<style>
</style>