<template>
  <div>
    <p>{{msg}}</p>
    <Test />
  </div>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';

import Test from './test.vue';

@Component({
  components: {
    Test: Test
  }
})
export default class App extends Vue {
  msg:string = 'Welcome to Your Vue.js App';
  
  data = function(){
    return {
      msg: "Hello World"
    }
  }
}
</script>