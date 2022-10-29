<template>
    <div id="header">
        <div class="logo">
            <div class="green_part">Э</div><div class="black_part">ко</div><div class="green_part">К</div><div class="black_part">вест</div>
        </div>
        <div class="role">Администратор</div>
        <div class="profile" @click="pop_up_profile_menu"><img src="@/assets/profile.png" alt=""></div>
        <div v-if="vis" id="profile_menu">
            <div id="gen_stat">
                Выгрузить статистику
            </div>
            <div id="change_pass">
                Сменить пароль
            </div>
            <div id="disk">
                <a href="https://45.86.181.125:9443/" target="_blank">Диск</a>
            </div>
            <div @click="logout" id="logout">
                Выйти
            </div>
        </div>
    </div>
    
</template>

<script>
export default {
  name: 'AdminHeader',
  data(){
    return {
        vis: false
    }
  },
  methods: {
      logout: function () {
          this.$emit('logout');
      },
      pop_up_profile_menu: function () {
          this.vis = !this.vis;
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
    height: 22%;
    left: 84.7%;
    top: 9.4%;
    position: absolute;
    background: white;
    border: solid black 2px;
    border-radius: 15px;
    font-size: 80%;
    z-index: 15;
}

#change_pass{
    margin-left: 8%;
}

#logout{
    margin-left: 8%;
    margin-top: 3%;
}
#gen_stat{
    margin-left: 8%;
}
#disk{
    margin-left: 8%;
}

#change_pass:hover, #logout:hover, #gen_stat:hover, #disk:hover {
    text-decoration: underline;
}
</style>
