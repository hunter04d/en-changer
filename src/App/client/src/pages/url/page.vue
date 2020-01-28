<template>
  <v-app id="page">
    <v-content>
      <v-container fluid>
        <v-card max-width="100vh" ripple class="mx-auto">
          <v-card-title>{{ password }}</v-card-title>
          <v-card-actions>
            <div class="flex-grow-1"></div>
            <v-btn text href="/">new link</v-btn>
          </v-card-actions>
        </v-card>
      </v-container>
    </v-content>
  </v-app>
</template>

<script>
export default {
  name: "Url",
  data() {
    return {
      password: ""
    };
  },
  mounted() {
    const id = window.location.pathname.substring(
      window.location.pathname.lastIndexOf("/") + 1
    );
    console.log(id);
    fetch(`/api/${id}`)
      .then(res => res.json())
      .then(data => (this.password = data.password));
  }
};
</script>
