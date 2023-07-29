<template>
    <div id="primary_grid">
        <div id="common_metrics">
            <div class="metrics">
                <div>Место в рейтинге:</div>
                <div>{{base_metrics.place}}</div>
            </div>
            <div class="metrics">
                <div>Всего игр сыграно:</div>
                <div>{{base_metrics.total_quiz}}</div>
            </div>
            <div class="metrics">
                <div>Ср. % верных ответов:</div>
                <div>{{base_metrics.percent_correct.toFixed(2) + '%'}}</div>
            </div>
            <!-- <div class="metrics">
                <div>Бейджей получено:</div>
                <div>{{base_metrics.badges}}</div>
            </div> -->
        </div>
        <div id="stat_group">
            <div id="filters">
                <div>Фильтры</div>
                <select class="filter" id="mode" @input="change_filter">
                    <option>Тренировочный режим</option>
                    <option>Случайный режим</option>
                    <option>Соревновательный режим</option>
                </select>
                <select class="filter" id="period" @input="change_filter">
                    <option>За всё время</option>
                    <option>За неделю</option>
                    <option>За месяц</option>
                    <option>За год</option>
                </select>
                <select class="filter" id="metric" @input="change_metric_filter">
                    <option>Процент верных ответов</option>
                    <option>Использование подсказок</option>
                    <option>Среднее время на ответ</option>
                    <option>Вопросов отвечено</option>
                    <option>Уникальных вопросов отвечено</option>
                </select>
                <select multiple class="filter" id="product" v-model="selectedItems" @change="change_product_filter" >
                    <option v-for="(product, index) in products" :key="index">{{product.name}}</option>
                </select>
            </div>
            <div id="chart">
                <Bar v-if="loaded"
                     id="my-chart-id"
                     :options="chartOptions"
                     :data="chartData"
                />
            </div>
        </div>
        <div v-if="!player_id" class="button" @click="to_menu">
            В меню
        </div>
    </div>
</template>

<script>
import { Bar } from 'vue-chartjs'
import { Chart as ChartJS, Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale } from 'chart.js'
import { SERVER_PATH } from '../common_const.js'

ChartJS.register(Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale);
if(window.innerWidth < 800)
    ChartJS.defaults.font.size = 14;
else
    ChartJS.defaults.font.size = 28;
ChartJS.defaults.color = '#000';

export default {
  name: 'PlayerStat',
  props:['player_id'],
  components: {
    Bar
  },
  data(){
    return {
        loaded: false,
        base_metrics: {place: 0, total_quiz: 0, percent_correct: 0, badges: 0},
        products: [],
        show_producs: [],
        selectedItems: [],
        raw_chart_data: [],
        mode_map: new Map([
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
        product_map: new Map(),
        values_map: new Map(),
        chartData: {
            labels: [],
            datasets: [ 
                { 
                    data: [],
                    backgroundColor: 'rgba(255, 159, 64, 0.5)',
                    borderColor: 'rgba(255, 159, 64)',
                } 
            ]
        },
        chartOptions: {
            // indexAxis: 'y',
            elements: {
                bar: {
                    borderWidth: 2,
                }
            },
            responsive: true,
            plugins: {
                title: {
                    display: true,
                    text: 'Процент верных ответов'
                },
                legend: {
                    display: false
                },
            }
        }
    }
  },
  methods:{
    to_menu: function () {
        this.$emit('switch-mode');
    },
    change_product_filter: async function () {
        this.loaded = false;
        this.chartData.labels = [];
        this.chartData.datasets[0].data = [];
        this.selectedItems.forEach(product => {
            this.chartData.labels.push(product);
            this.chartData.datasets[0].data.push(this.values_map.get(this.product_map.get(product)));
            });

        await this.$nextTick();
        this.loaded = true;
    },
    change_metric_filter: async function () {
        this.loaded = false;
        this.chartData.labels = [];
        this.chartData.datasets[0].data = [];
        const current_metric = document.getElementById("metric").value;
        this.raw_chart_data.forEach(product => {
            if(current_metric == "Процент верных ответов")
            {
                this.chartData.datasets[0].data.push(product.percentCorrect);
                this.values_map.set(product.productId, product.percentCorrect);
                this.chartOptions.plugins.title.text = "Процент верных ответов";
            }
            else if(current_metric == "Использование подсказок")
            {
                this.chartData.datasets[0].data.push(product.percentHelp);
                this.values_map.set(product.productId, product.percentHelp);
                this.chartOptions.plugins.title.text = "Использование подсказок";
            }
            else if(current_metric == "Вопросов отвечено")
            {
                this.chartData.datasets[0].data.push(product.totalQuiz);
                this.values_map.set(product.productId, product.totalQuiz);
                this.chartOptions.plugins.title.text = "Вопросов отвечено";
            }
            else if(current_metric == "Среднее время на ответ")
            {
                this.chartData.datasets[0].data.push(product.duration);
                this.values_map.set(product.productId, product.duration);
                this.chartOptions.plugins.title.text = "Среднее время на ответ";
            }
            else if(current_metric == "Уникальных вопросов отвечено")
            {
                this.chartData.datasets[0].data.push(product.uniqueAnswers);
                this.values_map.set(product.productId, product.uniqueAnswers);
                this.chartOptions.plugins.title.text = "Уникальных вопросов отвечено";
            }
            });

        this.products.forEach(product => {
            this.chartData.labels.push(product.name);
            });

        await this.$nextTick();
        this.loaded = true;
    },
    fill_product_map: function (products) {
        products.forEach(product => this.product_map.set(product.name, product.productId));
    },
    change_filter: async function () {
        this.loaded = false;
        let current_user_id = this.player_id ? this.player_id : JSON.parse(localStorage.getItem('user')).userId;
        fetch(SERVER_PATH + "/statistic/chart", {
                method: "POST",
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({  UserId: current_user_id,
                                        ProductId: this.product_map.get(document.getElementById("product").value),
                                        Interval: this.period_map.get(document.getElementById("period").value),
                                        Mode: this.mode_map.get(document.getElementById("mode").value)})
        }).then( res => res.json() ).then( data => {
            this.raw_chart_data = data;
            this.chartData.datasets[0].data = [];
            this.chartData.labels = [];
            this.products.forEach(product => {
                    this.chartData.labels.push(product.name);
                });
            data.forEach(stat_row => {
                this.chartOptions.plugins.title.text = "Процент верных ответов";
                this.chartData.datasets[0].data.push(stat_row.percentCorrect);
                this.values_map.set(stat_row.productId, stat_row.percentCorrect);
                });
            this.loaded = true;
            } );
    },
    change_player: async function () {
        let current_user_id = this.player_id ? this.player_id : JSON.parse(localStorage.getItem('user')).userId;
        console.log(this.player_id);
        console.log(current_user_id);
        fetch(SERVER_PATH + "/statistic/user", {
                method: "POST",
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({UserId: current_user_id, ProductId: -1, Interval: 'all', Mode: 'common'})
        }).then( res => res.json() ).then( data => {
            if(data.length > 0)
            {
                this.base_metrics = data[0];
                this.base_metrics.place = 0;
            }
            else
            {
                this.base_metrics.place = 0;
                this.base_metrics.total_quiz = 0;
                this.base_metrics.percent_correct = 0;
                this.base_metrics.badges = 0;
            }
            } );

        fetch(SERVER_PATH + "/statistic/user", {
                method: "POST",
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({UserId: -1, ProductId: -1, Interval: 'all', Mode: 'common'})
        }).then( res => res.json() ).then( data => {
            data.forEach((user, index) => {
                if(user.user_id == current_user_id)
                    this.base_metrics.place = index + 1;
                });
            } );
    },
  },
  beforeMount: async function () {
    fetch(SERVER_PATH + "/product/get/all/3", {
                method: "GET",
                headers: {'Content-Type': 'application/json'}
        }).then( res => res.json() ).then( data => {
            this.products = data;
            data.forEach(product => this.chartData.labels.push(product.name));
            this.fill_product_map(data);
            this.show_producs = data;
            if(data.length > 5 || window.innerWidth < 800)
                this.chartOptions.indexAxis = 'y'
            } );

    let current_user_id = this.player_id ? this.player_id : JSON.parse(localStorage.getItem('user')).userId;

    fetch(SERVER_PATH + "/statistic/user", {
                method: "POST",
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({UserId: current_user_id, ProductId: -1, Interval: 'all', Mode: 'common'})
        }).then( res => res.json() ).then( data => {
            this.base_metrics = data[0];
            this.base_metrics.place = 0;
            } );

    fetch(SERVER_PATH + "/statistic/user", {
                method: "POST",
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({UserId: -1, ProductId: -1, Interval: 'all', Mode: 'common'})
        }).then( res => res.json() ).then( data => {
            data.forEach((user, index) => {
                if(user.user_id == current_user_id)
                    this.base_metrics.place = index + 1;
                });
            } );

    fetch(SERVER_PATH + "/statistic/chart", {
                method: "POST",
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({UserId: current_user_id, ProductId: -1, Interval: 'all', Mode: 'train'})
        }).then( res => res.json() ).then( data => {
            this.raw_chart_data = data;
            data.forEach(stat_row => {
                this.chartData.datasets[0].data.push(stat_row.percentCorrect);
                this.values_map.set(stat_row.productId, stat_row.percentCorrect);
                });
            this.loaded = true;
            } );
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
#primary_grid{
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    text-align: center;
    font-size: 1.9vmax;
}

#common_metrics{
    margin-top: 2vmax;
    width: 100%;
    display: flex;
    flex-direction: row;
    justify-content: space-around;
    align-items: center;
    text-align: center;
}

@media screen and (max-width: 800px) {
  #common_metrics {
    flex-direction: column;
  }
}

#stat_group{
    width: 100%;
    margin-top: 3vmax;
    display: flex;
    flex-direction: row;
    justify-content: space-around;
    align-items: center;
    text-align: center;
}

@media screen and (max-width: 800px) {
  #stat_group {
    flex-direction: column-reverse;
  }
}

#filters{
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: left;
    text-align: center;
}

.filter{
    margin-top: 1vmax;
    font-size: 1.5vmax;
    padding: 0.5vmax 0 0.5vmax 0.5vmax;
}

.metrics{
    border: solid black 0.25vmax;
    border-radius: 1vmax;
    padding: 0.5vmax;
}

@media screen and (max-width: 800px) {
  .metrics {
    width: 80%;
  }
}

#chart{
    width: 50%;
    height: 50%;
}

@media screen and (max-width: 800px) {
  #chart {
    width: 95%;
  }
}

.button{
    width: 30vmax;
    margin-top: 3vmax;
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