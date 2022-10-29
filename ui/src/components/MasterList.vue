<template>
    <div id="masters" class="select_masters">
        <input type="search" placeholder="&#128269; Поиск ведущего" class="search_masters" v-model="search_condition"/>
        <div class="scroll">
            <div @click="select" class="option_masters" v-for="(option, index) in search_options" :key="index" v-bind:value="option.value" v-bind:id="option.value" v-bind:name="option.text">{{ option.text }}</div>
        </div>
    </div>
</template>

<script>

export default {
  name: 'MasterList',
  props: ['master_chosen', 'masters'],
  data(){
    return {
        selected: null,
        counter: 0,
        search_options: [
        ],
        search_condition: ""
    }
  },
  methods: {
        select: function (element) {
            if(this.master_chosen && this.selected == element.target)
            {
                this.$emit('master-chosen', false, this.selected);
                this.masters.forEach(function(item) {
                    var id = item.value;
                    var other_element = document.getElementById(id);
                    other_element.style.opacity = 1;
                });
                return;
            }
                
            if(!this.master_chosen)
            {
                this.masters.forEach(function(item) {
                    var id = item.value;
                    if(document.getElementById(id))
                    {
                        var other_element = document.getElementById(id);
                        if(other_element != element.target)
                        other_element.style.opacity = 0.5;
                    }
                    
                });
                this.selected = element.target;
                this.$emit('master-chosen', true, this.selected);
            }
            else
            {
                this.masters.forEach(function(item) {
                    var id = item.value;
                    if(document.getElementById(id))
                    {
                        var other_element = document.getElementById(id);
                        if(other_element != element.target)
                            other_element.style.opacity = 0.5;
                        else
                            element.target.style.opacity = 1;
                    }
                });
                
                this.selected = element.target;
                this.$emit('master-chosen', true, this.selected);
            }
        },
        unselect: function () {
        this.masters.forEach(function(item) {
            var id = item.value;
            if(document.getElementById(id))
            {
                var other_element = document.getElementById(id);
                other_element.style.opacity = 1;
            }
        });
    }
    },
    created: function () {
            this.counter = this.masters.length;
            this.search_options = this.masters;
        },
    watch: {
        search_condition: function () {
            if(this.search_condition.length == 0)
            {
                this.search_options = this.masters;
                this.search_condition = "";
            }
            else
                this.search_options = this.masters.filter(option => option.text.indexOf(this.search_condition) != -1);
        },
        masters: function () {
            this.search_options = this.masters;
        }
    }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.select_masters{
    float: left;
    width: 20%;
    text-align: center;
    height: 89%;
    border-right: 2px solid silver;
    margin-right: 1%;
}

.scroll{
    height: 90%;
    overflow: auto;
    -ms-overflow-style: none;
    scrollbar-width: none;
}

.scroll::-webkit-scrollbar {
    width: 0;
    height: 0;
}

.master_title{
    font-size: 1.5vw;
    font-weight: bold;
    height: 5%;
}

.search_masters{
    padding-top: 3%;
    padding-bottom: 3%;
    text-align: left;
    border-radius: 35px;
    background-color: green;
    margin-top: 5%;
    color: #ffffff;
    font-size: 1.2vw;
    width: 90%;
    text-indent: 5%;
}

.option_masters{
    padding-top: 3%;
    padding-bottom: 3%;
    text-align: center;
    border-radius: 35px;
    background-color: green;
    margin: 5%;
    color: #ffffff;
    font-size: 1vw;
    font-weight: bold;
}

.option_masters_opacity{
    padding-top: 3%;
    padding-bottom: 3%;
    text-align: center;
    border-radius: 35px;
    background-color: red;
    margin: 5%;
    color: #ffffff;
    font-size: 1vw;
    font-weight: bold;
    
    opacity: 0.5;
}

.option_masters:hover {
    box-shadow: 0 0 10px 100px orange inset;
}
</style>
