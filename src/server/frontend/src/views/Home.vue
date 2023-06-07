<template>
  <div class="p-grid">
    <div class="p-col-12 p-md-6 p-lg-3">A</div>
    <div class="p-col-12 p-md-6 p-lg-3">B</div>
    <div class="p-col-12 p-md-6 p-lg-3">C</div>
    <div class="p-col-12 p-md-6 p-lg-3">D</div>
  </div>
  <!-- <div class="card">
    <Message :severity="message.severity">{{ message.content }}</Message>
  </div> -->
</template>

<script>
import "primeflex/primeflex.css";

export default {
  created: function() {
    fetch("internal/home/info")
      .then((res) => res.json())
      .then((info) => {
        this.message = info.message;
      })
      .catch(console.log);
  },
  data: function() {
    return {
      message: {
        severity: "info",
        content: "Loading..",
      },
    };
  },
  methods: {},
};
</script>
