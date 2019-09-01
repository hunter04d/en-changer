import Vue from "vue";
import _ from "./page.vue";
import vuetify from "@/plugins/vuetify";

new Vue({
  vuetify,
  render: h => h(_)
}).$mount("#app");
