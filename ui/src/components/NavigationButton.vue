<template>
    <div id="fields_1" class="to_fields" @click="to_fields_1" style="border-radius:0.75vw 0 0 0.75vw;">1 раунд</div>
    <div id="fields_2" class="to_fields" @click="to_fields_2">2 раунд</div>
    <div id="fields_3" class="to_fields" @click="to_fields_3">Викторина</div>
    <div id="masters" class="to_fields" @click="to_masters" style="border-radius:0 0.75vw 0.75vw 0;">Ведущие</div>
</template>

<script>
export default {
  name: 'NavigationButton',
  methods: {
        to_fields_1: function () {
            document.getElementById('fields_1').style.opacity = 0.5;
            document.getElementById('fields_2').style.opacity = 1;
            document.getElementById('fields_3').style.opacity = 1;
            document.getElementById('masters').style.opacity = 1;
            this.$emit('fields-1');
        },
        to_fields_2: function () {
            document.getElementById('fields_1').style.opacity = 1;
            document.getElementById('fields_2').style.opacity = 0.5;
            document.getElementById('fields_3').style.opacity = 1;
            document.getElementById('masters').style.opacity = 1;
            this.$emit('fields-2');
        },
        to_fields_3: function () {
            document.getElementById('fields_1').style.opacity = 1;
            document.getElementById('fields_2').style.opacity = 1;
            document.getElementById('fields_3').style.opacity = 0.5;
            document.getElementById('masters').style.opacity = 1;
            this.$emit('fields-3');
        },
        to_masters: function () {
            document.getElementById('fields_1').style.opacity = 1;
            document.getElementById('fields_2').style.opacity = 1;
            document.getElementById('fields_3').style.opacity = 1;
            document.getElementById('masters').style.opacity = 0.5;
            this.$emit('masters');
        }
  },
  mounted: function () {
      document.getElementById('fields_1').style.opacity = 0.5;
      document.getElementById('fields_2').style.opacity = 1;
      document.getElementById('fields_3').style.opacity = 1;
      document.getElementById('masters').style.opacity = 1;
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.to_fields{
    height: 5%;
    background-color: green;
    text-align: center;
    font-weight: bold;
    color: #ffffff;
    font-size: 1.75vw;
    float: left;
    width: 24.7%;
    margin-left: 0.25%;
}

.to_fields:hover {
    box-shadow: 0 0 10px 100px orange inset;
}
</style>
