<template>
    <div id="round_1" class="to_fields" @click="to_round_1" style="border-radius:0.75vw 0 0 0.75vw;">1 раунд</div>
    <div id="round_2" class="to_fields" @click="to_round_2">2 раунд</div>
    <div id="games" class="to_fields" @click="to_games" style="border-radius:0 0.75vw 0.75vw 0;">Игры</div>
</template>

<script>
export default {
  name: 'TemplateMenu',
  methods: {
        to_round_1: function () {
            document.getElementById('round_1').style.opacity = 1;
            document.getElementById('round_2').style.opacity = 0.5;
            document.getElementById('games').style.opacity = 0.5;
            this.$emit('round-1');
        },
        to_round_2: function () {
            document.getElementById('round_1').style.opacity = 0.5;
            document.getElementById('round_2').style.opacity = 1;
            document.getElementById('games').style.opacity = 0.5;
            this.$emit('round-2');
        },
        to_games: function () {
            document.getElementById('round_1').style.opacity = 0.5;
            document.getElementById('round_2').style.opacity = 0.5;
            document.getElementById('games').style.opacity = 1;
            this.$emit('games');
        }
  },
  mounted: function () {
      document.getElementById('round_1').style.opacity = 1;
      document.getElementById('round_2').style.opacity = 0.5;
      document.getElementById('games').style.opacity = 0.5;
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
    width: 25.25%;
    margin-left: 0.25%;
    margin-bottom: 0.5%;
    margin-top: 1%;
}

.to_fields:hover {
    box-shadow: 0 0 10px 100px orange inset;
}
</style>
