<template>
    <MasterList ref="masters" @master-chosen="chose_master" :masters="masters" />
    <MasterRequests @go-back="go_back" @add-master="add_master" @remove-master="remove_master" @save-edit="save_edit" :master_chosen="master_chosen" :selected="selected" />
</template>

<script>
import MasterList from './MasterList.vue'
import MasterRequests from './MasterRequests.vue'
import { SERVER_PATH } from '../common_const.js'

export default {
  name: 'ManageMasters',
  data(){
    return {
      current_text: 'Игровые поля',
      master_chosen: false,
      selected: null,
      masters:[ ],

    }
  },
  components: {
    MasterList,
    MasterRequests
  }, 
  methods: {
    to_masters: function () {
          this.$emit('to-masters');
      },
    chose_master: function (master_chosen, selected) {
        this.master_chosen = master_chosen;
        this.selected = selected;
    },
    go_back: function () {
        this.master_chosen = false;
        this.$refs.masters.unselect();
    },
    add_master: function(user){
      this.masters.push(user);
    },
    remove_master: function(){
      this.masters = this.masters.filter(option => option.userId != this.selected.userId);
      this.master_chosen = false;
      this.$refs.masters.unselect();
      fetch(SERVER_PATH + "/user/delete/" + String(this.selected.userId), {
                method: "DELETE",
                headers: {'Content-Type': 'application/json'}
                });
    },
    set_draw: function(draw){
      this.draw = draw;
    },
    log_out: function(){
        this.$emit('logout');
    },
    save_edit: function(first, middle, last, login){
      this.selected.firstName = first;
      this.selected.patronymic = middle;
      this.selected.lastName = last;
      this.selected.login = login;
      fetch(SERVER_PATH + "/user/update/info", {
          method: "POST",
          headers: {'Content-Type': 'application/json'},
          body: JSON.stringify(this.selected)
      });
    },
  },
  beforeMount: function () {
      let masters_ref = this.masters;
      masters_ref.length = 0;
    this.$nextTick(function () {

    fetch(SERVER_PATH + "/user/get/activeMasters", {
            method: "GET",
            headers: {'Content-Type': 'application/json'}
            }).then( res => res.json() ).then( data => data.forEach(function(item) {
                   masters_ref.push(item)}) );
    });
  }
}
</script>

<style scoped>
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
    height: 90%;
}
#app{
  height: 100%;
    width: 100%;
}

#team_1{
  position: absolute;
  left: 1%;
}

#team_2{
  position: absolute;
  left: 74%;
}

#team_3{
  position: absolute;
  left: 1%;
  top: 63%;
}

#team_4{
  position: absolute;
   top: 63%;
  left: 74%;
}
</style>
