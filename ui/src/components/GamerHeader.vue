<template>
    <div id="header">
        <div class="logo">
            <div class="green_part">Э</div><div class="black_part">ко</div><div class="green_part">К</div><div class="black_part">вест</div>
        </div>
        <div class="role">{{user_name()}}</div>
        <div class="profile" @click="vis = !vis"><img src="@/assets/profile.png" alt=""></div>
        <div v-if="vis" id="profile_menu">
            <div id="change_pass">
                Статистика
            </div>
            <div id="change_pass" @click="change_pass">
                Сменить пароль
            </div>
            <div @click="logout" id="logout">
                Выйти
            </div>
        </div>
    </div>
    
</template>

<script>
export default {
  name: 'GamerHeader',
  data(){
    return {
        vis: false
    }
  },
  methods: {
      change_pass: function () {
          this.$emit('change-pass');
      },
      logout: function () {
          this.$emit('logout');
      },
      pop_up_profile_menu: function () {
          this.vis = !this.vis;
      },
      user_name: function () {
          return JSON.parse(localStorage.getItem('user')).name;
      },
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
#header {
    width: 98%;
    background-color: #ffffff;
    border-bottom: 2px solid silver;
    padding: 1%;
    height: 5%;
    font-size: 2.2vw;
    color: #000000;
    margin-bottom: 0.1%;
}

.logo {
    float: left;
    width: 33.33%;
}
.green_part{
    float: left;
    color: rgb(33, 160, 56);
}
.black_part{
    float: left;
    color: #000000;
}

.role {
    float: left;
    text-align: center;
    width: 33.33%;
}

.profile {
    float: left;
    text-align: right;
    width: 33.33%;
}

.profile img {
    float: right;
    width: 9%;
    height: 9%;
    margin-right: 5%;
}

#profile_menu{
    width: 15%;
    height: 15%;
    left: 84.7%;
    top: 9.4%;
    position: absolute;
    background: white;
    border: solid black 2px;
    border-radius: 15px;
    font-size: 80%;
}

#change_pass{
    margin-left: 8%;
    margin-top: 2%;
}

#logout{
    margin-left: 8%;
    margin-top: 2%;
}

#change_pass:hover, #logout:hover {
    text-decoration: underline;
}
</style>
