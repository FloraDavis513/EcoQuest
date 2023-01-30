<template>
    <div id="header">
        <div class="logo">
            <div class="green_part">Э</div><div class="black_part">ко</div><div class="green_part">К</div><div class="black_part">вест</div>
        </div>
        <div class="role">Администратор</div>
        <div class="profile"><img src="@/assets/profile.png" @mouseenter="vis = true"></div>
        <div v-if="vis" id="profile_menu" @mouseleave="vis = false">
            <div id="gen_stat" @click="download_products" style="position:relative;float:left;margin-left: 8%;">
                Выгрузить продукты
            </div>
            <div id="gen_stat" @click="upload_products" style="position:relative;float:left;height:24%;margin-left: 8%;">
                <div id="gen_prod" style="position:absolute;margin-left:0;">Загрузить продукты</div>
                <input id="selected_logo" type="file" name="uploads" style="width:100%;height:100%;opacity: 0;" @change="upload_logo" @mouseenter="underline_up" @mouseleave="underline_down">
            </div>
            <div id="gen_stat" @click="download_stat" style="position:relative;float:left;">
                Выгрузить статистику
            </div>
            <div id="change_pass" style="position:relative;float:left;">
                Сменить пароль
            </div>
            <div @click="logout" id="logout" style="position:relative;float:left;">
                Выйти
            </div>
        </div>
    </div>
    
</template>

<script>
import { SERVER_PATH } from '../common_const.js'

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
            console.log('focus');
          this.vis = !this.vis;
      },
      underline_up: function () {
            document.getElementById('gen_prod').style.textDecoration = 'underline';
      },
      underline_down: function () {
            document.getElementById('gen_prod').style.textDecoration = 'none';
      },
      download_stat: async function () {
        await fetch(SERVER_PATH + "/statistic/export", {
            method: "POST",
            headers: {"Content-Type": "application/json"},
            body: JSON.stringify({})
    //         }).then(res => res.blob()).then(file => {
    //     console.log(file);
    //     const newBlob = new Blob([file], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
    //     let tempUrl = URL.createObjectURL(newBlob);
    //     const aTag = document.createElement("a");
    //     aTag.href = tempUrl;
    //     aTag.download = 'stat.xlsx';
    //     document.body.appendChild(aTag);
    //     aTag.click();
    //     URL.revokeObjectURL(tempUrl);
    //     aTag.remove();
    // }).catch(() => {
    //     alert("Failed to download file!");
    });
    }
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
    height: 35%;
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
