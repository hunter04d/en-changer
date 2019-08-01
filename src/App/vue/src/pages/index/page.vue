<template>
  <div id="page">
    <h1 class="md-display-3">EnChanger</h1>
    <div class="md-layout md-alignment-center-center">
      <div class="md-layout-item md-size-15 md-small-hide"></div>
      <div class="md-layout-item">
        <md-field>
          <md-icon>mode_edit</md-icon>
          <label>Password</label>
          <md-input v-model="password"></md-input>
        </md-field>
      </div>
      <div class="md-layout-item md-size-15 md-small-hide"></div>
    </div>
    <md-button
      :disabled="buttonDisabled"
      class="centre md-primary md-raised "
      v-on:click="onClick"
    >
      Encode
    </md-button>
    <div v-if="url" class="md-layout">
      <div class="md-layout-item md-size-15 md-small-hide"></div>
      <md-list class="md-elevation-1 md-layout-item">
        <md-list-item>
          <md-icon>link</md-icon>
          <a :href="url" class="md-list-item-text">{{ url }} </a>
        </md-list-item>
      </md-list>
      <div class="md-layout-item md-size-15 md-small-hide"></div>
    </div>
  </div>
</template>

<script>
export default {
  name: "index",
  data() {
    return {
      password: "",
      url: "",
      disabled: false
    };
  },
  computed: {
    buttonDisabled() {
      return !this.password || this.disabled;
    }
  },
  methods: {
    onClick() {
      this.disabled = true;
      this.url = "";
      if (this.password) {
        fetch("/api", {
          body: JSON.stringify({
            password: this.password
          }),
          headers: new Headers({ "content-type": "application/json" }),
          method: "post"
        })
          .then(res => res.headers.get("Location"))
          .then(str => {
            this.disabled = false;
            this.url = str.replace("api", "url");
            console.log(str);
          });
      }
    }
  }
};
</script>

<style lang="scss">
#page > h1 {
  text-align: center;
}

.centre {
  display: block !important;
  margin-left: auto !important;
  margin-right: auto !important;
}
</style>
