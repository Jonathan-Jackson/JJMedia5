<template>
  <div v-if="isLoading">
    <ProgressSpinner />
  </div>
  <transition name="fade">
    <Home v-if="isSetup" />
    <Setup v-else-if="!isLoading" />
    <div v-else></div>
  </transition>
</template>

<script>
import Setup from "./views/Setup.vue";
import Home from "./views/Home.vue";

export default {
  components: { Setup, Home },
  data: function() {
    return {
      isSetup: false,
      isLoading: true,
      show: true,
    };
  },
  created: function() {
    fetch("api/v1/start/status")
      .then((res) => res.json())
      .then((info) => {
        this.isSetup = info.isSetup;
        this.isLoading = false;
      });
  },
  methods: {},
};
</script>

<style>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 1.5s;
}
.fade-enter,
.fade-leave-to {
  opacity: 0;
}
</style>
