<script>
import AuthForm from './AuthForm.vue'
import FieldsList from './FieldsList.vue'
import MasterTemplates from './MasterTemplates.vue'
import TheGame from './TheGame.vue'
import GameLobby from './GameLobby.vue'
import MasterPanel from './MasterPanel.vue'
import RegistrationForm from './RegistrationForm.vue'
import GamerMenu from './GamerMenu.vue'
import TheQuiz from './TheQuiz.vue'

const routes = {
  '/': AuthForm,
  '/auth/login': AuthForm,
  // '/admin': FieldsList,
  '/fields': FieldsList,
  '/templates': MasterTemplates,
  '/game': TheGame,
  '/lobby': GameLobby,
  '/status': MasterPanel,
  '/auth/registration': RegistrationForm,
  '/gamer': GamerMenu,
  '/quiz': TheQuiz,
}

export default {
  name: 'MainApp',
  data() {
    return {
      currentPath: window.location.hash
    }
  },
  methods: {
    login_admin() {
      // window.location.href = "#/admin";
      window.location.href = "#/fields";
      this.currentPath = window.location.hash
    },
    login_master() {
      window.location.href = "#/templates";
      this.currentPath = window.location.hash
    },
    to_fields() {
      // window.location.href = "#/admin";
      window.location.href = "#/fields";
      this.currentPath = window.location.hash
    },
    login_player() {
      window.location.href = "#/gamer";
      this.currentPath = window.location.hash
    },
    log_out() {
      window.location.href = "#/auth/login";
      this.currentPath = window.location.hash
    },
    start_game() {
      window.location.href = "#/game";
      this.currentPath = window.location.hash;
    },
    create_game(game_id) {
      window.location.href = "#/lobby";
      this.currentPath = window.location.hash;
      this.game_id = game_id;
    },
    already_registered() {
      window.location.href = "#/auth/login";
      this.currentPath = window.location.hash
    },
    go_to_reg() {
      window.location.href = "#/auth/registration";
      this.currentPath = window.location.hash
    },
    start_quiz(quiz_products) {
      window.location.href = "#/quiz";
      this.currentPath = window.location.hash;
      this.quiz_products = quiz_products;
    },
  },
  computed: {
    currentView() {
      return routes[this.currentPath.slice(1) || '/']
    }
  },
  mounted() {
    window.addEventListener('hashchange', () => {
            this.currentPath = window.location.hash
		});
  }
}
</script>

<template>
  <component ref="component" :is="currentView" :game_id="game_id" :quiz_products="quiz_products" @login-admin="login_admin" @login-master="login_master" @to-fields="to_fields" @to-masters="to_masters" @logout="log_out" @create-game="create_game" @start-game="start_game" @already-registered="already_registered" @go-to-reg="go_to_reg" @login-player="login_player" @start-quiz="start_quiz" />
</template>

<style>
@import url('http://fonts.cdnfonts.com/css/menlo');
#app{
  height: 100%;
  width: 100%;
}
html {
    height: 100%;
    width: 100%;
    overflow: auto;
    -ms-overflow-style: none;
    scrollbar-width: none;
    font-family: 'Roboto Light', sans-serif;
}

body {
    width: 99%;
    height: 95%;
}
</style>