<template>
  <div>
    <p>{{msg}}</p>
    <rktree />
    <rktextview />
  </div>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';

import RekoTreeView from "./rk-tree.vue";
import RekoTextView from "./rk-textview.vue";
import RekoTreeItem from './rk-treeitem.vue';

import Test from './test.vue';

@Component({
  components: {
    rktree: RekoTreeView,
    rktextview: RekoTextView,
    rktreeitem: RekoTreeItem,
    rktest: Test
  }
})
export default class RekoApp extends Vue {
  msg:string = 'Welcome to Your Vue.js App';
  
  data = function(){
    return {
      msg: "Hello World"
    }
  }

  public procedures(arg:any){
    //TODO
  }

}
</script>

<style scoped>
@import "./style.css";
@import "~bootstrap.min.css";
@import "~element-default.css";
</style>