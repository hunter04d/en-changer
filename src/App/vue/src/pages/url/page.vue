<template>
  <div id="page" class="md-layout md-alignment-center-space-around md-gutter">
    <div class="md-layout-item md-small-hide"> </div>
    <div class="md-layout-item">
      <md-card md-with-hover>
        <md-card-header>
          <div class="md-title">{{ password }}</div>
        </md-card-header>
        <md-card-actions>
          <md-button href="/">Generate new link</md-button>
        </md-card-actions>
      </md-card>
    </div>
    <div class="md-layout-item md-small-hide"></div>
  </div>
</template>

<script>
export default {
  name: "url",
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

<style scoped lang="scss"></style>
