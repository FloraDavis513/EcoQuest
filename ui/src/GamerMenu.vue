<template>
    <GamerHeader @logout="log_out" @change-pass="change_pass = true" />
    <ModeSwitcher v-if="menu == 'menu'" @pick-themes="pick_themes" @show-help="show_help" @start-quiz="start_quiz"  />
    <ThemesPicker v-if="menu == 'themes'" @switch-mode="switch_mode" @start-quiz="start_quiz" />
    <div v-if="menu == 'help'" id="help" @click="switch_mode">
      <div>
          Справка
      </div>
      <div style="width:80%;margin-top:1vmax;">
        Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum
      </div>
      <div class="button">
        Назад
      </div>
    </div>
</template>

<script>
import GamerHeader from './components/GamerHeader.vue'
import ModeSwitcher from './components/ModeSwitcher.vue'
import ThemesPicker from './components/ThemesPicker.vue'

export default {
  name: 'GamerMenu',
  components: {
    GamerHeader,
    ModeSwitcher,
    ThemesPicker
  }, 
  data(){
    return {
      menu: 'menu'
    }
  },
  methods:{
    pick_themes: function () {
        this.menu = 'themes';
    },
    switch_mode: function () {
        this.menu = 'menu';
    },
    show_help: function () {
        this.menu = 'help';
    },
    start_quiz: function (products) {
        this.$emit('start-quiz', products);
    },
    log_out: function () {
        this.$emit('log_out');
    },
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
#help{
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    text-align: center;
    font-size: 1.9vmax;
    margin-top: 1vmax;
}

.button{
    width: 30vmax;
    margin-top: 1vmax;
    background-color: green;
    color: #ffffff;
    font-weight: bold;
    border-radius: 1.5vmax;
    padding-top: 1vmax;
    padding-bottom: 1vmax;
    text-align: center;
}

.button:hover {
    box-shadow: 0 0 10px 100px orange inset;
}
</style>