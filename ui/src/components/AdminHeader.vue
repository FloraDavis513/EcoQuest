<template>
    <div id="header">
        <div class="logo">
            <div class="green_part">Э</div><div class="black_part">ко</div><div class="green_part">К</div><div class="black_part">вест</div>
        </div>
        <div class="role">{{user_name() + " (администратор)"}}</div>
        <div id="updated_message" style="width:20%;float:left;text-align:right;font-size:1.5vw;margin-top:0.5%;color:white;">Продукты обновлены</div>
        <div class="profile"><img src="@/assets/profile.png" @click="vis = !vis"></div>
        <div v-if="vis" id="profile_menu">
            <div id="gen_stat" @click="download_products" style="position:relative;float:left;margin-left: 8%;">
                Выгрузить продукты
            </div>
            <div id="gen_stat" style="position:relative;float:left;height:18%;margin-left: 8%;">
                <div id="gen_prod" style="position:absolute;margin-left:0;">Загрузить продукты</div>
                <input id="selected_logo" type="file" name="uploads" style="width:100%;height:100%;opacity: 0;" @change="upload_products" @mouseenter="underline_up" @mouseleave="underline_down">
            </div>
            <div id="gen_stat" @click="download_stat" style="position:relative;float:left;">
                Выгрузить статистику
            </div>
            <div id="gen_stat" @click="download_quiz_stat" style="position:relative;float:left;">
                Выгрузить статистику викторин
            </div>
            <div id="change_pass" style="position:relative;float:left;" @click="change_pass">
                Сменить пароль
            </div>
            <div @click="logout" id="logout" style="position:relative;float:left;">
                Выйти
            </div>
        </div>
    </div>
    
</template>

<script>
import { SERVER_PATH, SRC_PATH } from '../common_const.js'

export default {
  name: 'AdminHeader',
  data(){
    return {
        vis: false
    }
  },
  methods: {
      upload_products: async function () {
        this.vis = true;
        const uploadFileEle = document.getElementById("selected_logo");
        let file = uploadFileEle.files[0];
        let formData = new FormData();
        formData.set('uploads', file);
        await fetch(SERVER_PATH + "/product/import", {
            method: "POST",
            body: formData
        }).then(res => {
            let secs = 5;
            function tick(){
                (--secs);
                if(secs == 0)
                {
                    clearInterval(timer);
                    document.getElementById('updated_message').style.color = 'white';
                }
            }
            if(res)
            {
                var timer = setInterval(tick, 1000)
                document.getElementById('updated_message').style.color = 'red';
            }
        });
        this.$emit('reload-question');
      },
      change_pass: function () {
          this.$emit('change-pass');
      },
      logout: function () {
          this.$emit('logout');
      },
      pop_up_profile_menu: function () {
          this.vis = !this.vis;
      },
      underline_up: function () {
            document.getElementById('gen_prod').style.textDecoration = 'underline';
      },
      underline_down: function () {
            document.getElementById('gen_prod').style.textDecoration = 'none';
      },
      download_products: async function () {
        let current_date = new Date();
        let file_name = `product_${current_date.getDate()}_${current_date.getMonth() + 1}_${current_date.getFullYear()}`;
        await fetch(SERVER_PATH + "/product/export", {
            method: "POST",
            headers: {"Content-Type": "application/json"},
            body: JSON.stringify({FileName: file_name})
        });
        this.start_download(SRC_PATH + file_name + '.xlsx');
    },
      download_stat: async function () {
        let current_date = new Date();
        let file_name = `statistics_${current_date.getDate()}_${current_date.getMonth() + 1}_${current_date.getFullYear()}`;
        await fetch(SERVER_PATH + "/statistic/export", {
            method: "POST",
            headers: {"Content-Type": "application/json"},
            body: JSON.stringify({FileName: file_name})
        });
        this.start_download(SRC_PATH + file_name + '.xlsx');
    },
    download_quiz_stat: async function () {
        let current_date = new Date();
        let file_name = `quiz_statistics_${current_date.getDate()}_${current_date.getMonth() + 1}_${current_date.getFullYear()}`;
        await fetch(SERVER_PATH + "/statistic/players/export", {
            method: "POST",
            headers: {"Content-Type": "application/json"},
            body: JSON.stringify({FileName: file_name})
        });
        this.start_download(SRC_PATH + file_name + '.xlsx');
    },
    start_download: function (href) {
        const link = document.createElement('a')
        link.setAttribute('href', href);
        link.style.display = 'none'
        document.body.appendChild(link)
        link.click()
        document.body.removeChild(link)
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
    width: 25%;
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
    width: 50%;
}

.profile {
    float: left;
    text-align: right;
    width: 5%;
}

.profile img {
    float: right;
    width: 60%;
    height: 60%;
    margin-right: 5%;
}

#profile_menu{
    width: 15%;
    height: 50%;
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
