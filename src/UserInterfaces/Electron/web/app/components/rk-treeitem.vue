<template>
	<div>
		<i class="el-icon-info"></i>
		<span>{{ label }}</span>
	</div>
</template>

<script lang="ts">
	import Vue from 'vue';
	import Component from 'vue-class-component';

	@Component({
		props: {
			label: String
		}
	})
	export default class RekoTreeItem extends Vue {
		label!:String;
	}
</script>

<style scoped>
</style>
