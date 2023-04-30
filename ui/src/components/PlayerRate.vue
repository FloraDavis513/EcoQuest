<template>
    <div id="primary_grid">
        <div id="stat_group">
            <div id="stat_header">
                <div id="rank">{{"Ваше место: " + user_place}}</div>
                <div id="filters">
                    <select class="filter" id="mode" @input="change_filter">
                        <option selected>Общий рейтинг</option>
                        <option>Тренировочный режим</option>
                        <option>Случайный режим</option>
                        <option>Соревновательный режим</option>
                    </select>
                    <select class="filter" id="period" @input="change_filter">
                        <option selected>За неделю</option>
                        <option>За месяц</option>
                        <option>За год</option>
                        <option>За всё время</option>
                    </select>
                    <select class="filter" id="product" @input="change_filter">
                        <option selected>Все продукты</option>
                        <option v-for="(product, index) in products" :key="index">{{product.name}}</option>
                    </select>
                </div>
            </div>
            <div id="leaderboard">
                <Vue3EasyDataTable
                    :rows-items="[15]"
                    :rows-per-page="15"
                    :headers="headers"
                    :items="items"
                    :theme-color="theme_color"
                    table-class-name="customize-table"
                    alternating
                    buttons-pagination
                    hide-rows-per-page
                    :loading="is_loading"
                    border-cell
                    header-text-direction="center"
                    body-text-direction="center">
                    <template #empty-message>
                        <div style="font-size:1.5vw;">Нет данных, удовлетворяющих текущему фильтру</div>
                    </template>
                </Vue3EasyDataTable>
            </div>
        </div>
        <div class="button" @click="to_menu">
            В меню
        </div>
    </div>
</template>

<script>
import Vue3EasyDataTable from 'vue3-easy-data-table';
import 'vue3-easy-data-table/dist/style.css';
import { SERVER_PATH } from '../common_const.js'

export default {
  name: 'PlayerRate',
  components: {
    Vue3EasyDataTable
  },
  data(){
    return {
        is_loading: false,
        user_place: "",
        theme_color: "#008000",
        mode_map: new Map([
            ['Общий рейтинг', 'common'],
            ['Тренировочный режим', 'train'],
            ['Случайный режим', 'random'],
            ['Соревновательный режим', 'challenge'],
        ]),
        period_map: new Map([
            ['За всё время', 'all'],
            ['За неделю', 'week'],
            ['За месяц', 'month'],
            ['За год', 'year'],
        ]),
        product_map: new Map([['Все продукты', -1]]),
        products: [],
        headers: [
            { text: "Место", value: "place" },
            { text: "Имя", value: "name", sortable: true },
            { text: "Игр сыграно", value: "total_quiz" },
            { text: "Бейджи", value: "badges" },
            { text: "Верных ответов, %", value: "percent_correct", sortable: true },
            { text: "Ср. время на ответ, с", value: "duration", sortable: true },
            { text: "Частота использования подсказок, %", value: "percent_help", sortable: true },
        ],
        items: []
    }
  },
  methods:{
    to_menu: function () {
        this.$emit('switch-mode');
    },
    change_filter: async function () {
        this.is_loading = true;
        fetch(SERVER_PATH + "/statistic/user", {
                method: "POST",
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({  UserId: -1, 
                                        ProductId: this.product_map.get(document.getElementById("product").value),
                                        Interval: this.period_map.get(document.getElementById("period").value),
                                        Mode: this.mode_map.get(document.getElementById("mode").value)})
        }).then( res => res.json() ).then( data => {
            this.items = data;
            this.items.sort((a, b) => {
                a.total_quiz + a.percent_correct + (100 - a.percent_help) + a.badges > b.total_quiz + b.percent_correct + (100 - b.percent_help) + b.badges;
            });
            this.items.forEach((item, index) =>{
                item.place = index + 1;
                if(JSON.parse(localStorage.getItem('user')).userId == item.user_id)
                    this.user_place = index + 1;
            });
            this.is_loading = false;
            } );
    },
    fill_product_map: function (products) {
        products.forEach(product => this.product_map.set(product.name, product.productId));
    },
  },
  beforeMount: async function () {
    this.is_loading = true;
    fetch(SERVER_PATH + "/statistic/user", {
                method: "POST",
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({UserId: -1, ProductId: -1, Interval: 'week', Mode: 'common'})
        }).then( res => res.json() ).then( data => {
            this.items = data;
            this.items.sort((a, b) => {
                a.total_quiz + a.percent_correct + (100 - a.percent_help) + a.badges > b.total_quiz + b.percent_correct + (100 - b.percent_help) + b.badges;
            });
            this.items.forEach((item, index) =>{
                item.place = index + 1;
                if(JSON.parse(localStorage.getItem('user')).userId == item.user_id)
                    this.user_place = index + 1;
            });
            this.is_loading = false;
            } );
    fetch(SERVER_PATH + "/product/get/all/3", {
                method: "GET",
                headers: {'Content-Type': 'application/json'}
        }).then( res => res.json() ).then( data => {
            this.products = data;
            this.fill_product_map(data);
            } );
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
#primary_grid{
    width: 100%;
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    text-align: center;
    font-size: 1.9vmax;
}

#stat_group{
    width: 100%;
    /* margin-top: 3vmax; */
    display: flex;
    flex-direction: column;
    justify-content: space-around;
    align-items: center;
    text-align: center;
}

#stat_header{
    width: 100%;
    display: flex;
    flex-direction: row;
    justify-content: start;
    align-items: left;
    text-align: center;
}

#filters{
    width: 70%;
    display: flex;
    flex-direction: row;
    justify-content: space-evenly;
    align-items: right;
    text-align: center;
}

.filter{
    margin-top: 1vmax;
    font-size: 1.5vmax;
    padding: 0.5vmax 0 0.5vmax 0.5vmax;
}

#leaderboard{
    margin-top: 2vmax;
    width: 95%;
    height: 30vmax;
    /* height: 100%; */
}

.button{
    width: 30vmax;
    margin-top: 3vmax;
    background-color: #008000;
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

#rank{
    margin-left: 2.5%;
    margin-top: 1vmax;
    width: 25%;
    border: solid black 0.25vmax;
    border-radius: 1vmax;
    padding: 0.5vmax;
}

.customize-table {
    --easy-table-header-font-color: #fff;
    --easy-table-header-background-color: #008000;
    --easy-table-footer-font-size: 0.75vmax;
    --easy-table-header-font-size: 1.5vmax;
    --easy-table-body-row-font-size: 1.5vmax;
}
</style>