<template>
    <div class="scroll">
     <div class="grid" id="grid">
        <div v-for="(option, index) in products" :key="index" class="cell">
            <div  class="weight" :style="'background-color: ' + option.weight"></div>
            <div class="grid_element"  @click="select_product(index)"  v-bind:style="option.colour">
                {{ option.name }}
            </div>
        </div>
        <div class="cell">
            <div class="weight" style="background-color:white;"></div>
            <div class="grid_element plus" @click="add_product">+</div>
        </div>
     </div>
    </div>
</template>

<script>
export default {
  name: 'FieldList',
  props:['products'],
  data(){
    return {
    }
  },
  methods: {
        add_product: function () {
            this.$emit('add-field');
        },
        select_product: function (index) {
            this.$emit('select-product', this.products[index]);
        }
  },
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.grid{
    margin-top: 3%;
    margin-left: 5%;
    width: 90%;
    height: 80%;
}

.grid_element{
    margin-bottom: 5%;
    float: left;
    width: 80%;
    height: 100%;
    border: 0.15vw solid black;
    border-radius: 1vw;
    text-align: center;
    font-size: 220%;
    font-size: 1.8vw;
    line-height: 100px;
    line-height: 6.3vw;
    color: white;
    transition: transform .25s ease;
}

.grid_element:hover {
  transform: scale(1.1); /* (150% zoom - Note: if the zoom is too large, it will go outside of the viewport) */
}

.number{
    color: black;
    font-size: 2vw;
}

.plus{
    font-size: 6vw;
    border: 2px solid silver;
    color: silver;
}

.scroll{
    height: 86%;
    overflow: auto;
    -ms-overflow-style: none;
    scrollbar-width: none;
    margin-top: 1%;
    width: 100%;
}

.scroll::-webkit-scrollbar {
    width: 0;
    height: 0;
}

.cell{
    float: left;
    width: 20%;
    margin-bottom: 2.5%;
    display: flex;
    flex-direction: column;
    justify-content: space-around;
    align-items: center;
    text-align: center;
}

.weight{
    width: 50%;
    height: 0.35vmax;
    margin-bottom: 2.5%;
    float: left;
}
</style>
