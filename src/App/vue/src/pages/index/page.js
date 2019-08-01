import Vue from "vue";
import _ from "./page.vue";
import Material from "vue-material";
import "vue-material/dist/vue-material.min.css";
import "vue-material/dist/theme/default.css";
Vue.use(Material);

new Vue({
  render: h => h(_)
}).$mount("#app");
