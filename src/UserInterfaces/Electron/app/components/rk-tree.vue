<template>
	<el-tree
		:data="data"
		:props="defaultProps"
		:render-content="renderContent"
		@node-click="handleNodeClick"
	></el-tree>
</template>

<script lang="ts">
	import Vue from 'vue';
	import Component from 'vue-class-component';
	import { VueConstructor, CreateElement } from 'vue/types/vue';
	import { VueClass } from 'vue-class-component/lib/declarations';
	
	
	import { createDecorator } from 'vue-class-component/lib/util';
	import { VNode } from 'vue/types/vnode';
	import { TreeNode } from 'element-ui/types/tree';

	import RekoTreeItem from "./rk-treeitem.vue";

	@Component({})
	export default class RekoTreeView extends Vue {
		data = [{
				label: "Hello",
				children: [{
					label: "Nested"
				}]
			}, {
				label: "World"
			}];

		defaultProps = {
			label: "label"
		};

		handleNodeClick(data:any){
			console.log(data.label);
		}

		renderContent(h: CreateElement, data: { node: TreeNode }): VNode {
			return h(RekoTreeItem, {
				props: {
					label: data.node.label
				}
			});
		}

	}
</script>

<style scoped>
</style>