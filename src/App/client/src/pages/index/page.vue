<template>
  <v-app id="page">
    <v-content>
      <v-container fluid>
        <h1 class="display-1 text-center">EnChanger</h1>

        <v-row>
          <v-col class="offset-md-2" md="8">
            <v-text-field
              v-model="password"
              label="Information"
              prepend-icon="mdi-pencil"
              outlined
            />
            <v-btn
              block
              rounded
              x-large
              :disabled="buttonDisabled"
              @click="onClick"
            >
              Encode
            </v-btn>
            <v-banner v-if="url" class="mt-2" elevation="2" single-line>
              <a :href="url">{{ url }} </a>
            </v-banner>
          </v-col>
        </v-row>
      </v-container>
    </v-content>
  </v-app>
</template>

<script>
export default {
  name: "Index",
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
