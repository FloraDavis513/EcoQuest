<head>
    <meta charset="UTF-8">
    <title>Skills</title>
    <link rel="stylesheet" href="main.css">
</head>
<template>  
    <div class="section">
        <div class="teamImage">
            <img src="@/assets/crown.png" class="ms-auto" width="120" height="120"/>
        </div>
        <div class="skills">Результаты</div>
        
        <div class="diagram" id="teams">
            <div class="skillBLock">
                <div class="teamImage">
                    <img src="@/assets/team_logo_1.png" class="ms-auto" />
                </div>
                <div class="column">
                    <span>25</span>

                </div>
            </div>
            <div class="skillBLock">
                <div class="teamImage">
                    <img src="@/assets/team_logo_2.png" class="ms-auto" />
                </div>
                <div class="column">
                    <span>30</span>
                </div>
            </div>
            <div class="skillBLock">
                <div class="teamImage">
                    <img src="@/assets/team_logo_3.png" class="ms-auto" />
                </div>
                <div class="column">
                    <span>40</span>
                </div>
            </div>
            <div class="skillBLock">
                <div class="teamImage">
                    <img src="@/assets/team_logo_4.png" class="ms-auto" />
                </div>
                <div class="column">
                    <span>20</span>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
   
    

    
</script>

<style>
    .teamImage {
        display: block;
        padding-bottom: 20px;
        
    }
    .section {
        position: absolute;
        left: 50%;
        top: 50%;
        transform: translate(-50%, -50%);
        display: flex;
        align-items: center;
        flex-direction: column;
        width: 25%;
    }

    .skills {
        font-weight: bold;
        text-transform: uppercase;
        color: black;
        font-size: 50px;
        margin-bottom: 30px;
    }

    .diagram {
        display: flex;
        justify-content: space-around;
        align-items: center;
        width: 100%;
    }

    .skillBLock {
        height: 300px;
        display: flex;
        justify-content: flex-end;
        align-items: center;
        flex-direction: column;
    }

    .column {
        display: flex;
        justify-content: center;
        align-items: center;
        width: 235px;
    }

    .skillBLock:first-child .column {
        background: #d45dff;
        height: calc(350px / 100 * 25); 
        animation: appFir 2s 1;
    }

    .skillBLock:nth-child(2) .column {
        background: #4cff00;
        height: calc(350px / 100 * 30); 
        animation: appSec 2s 1;
    }

    .skillBLock:nth-child(3) .column {
        background: #f20c6c;
        height: calc(350px / 100 * 40); 
        animation: appThi 2s 1;
    }

    .skillBLock:last-child .column {
        background: #b50cf2;
        height: calc(350px / 100 * 20); 
        animation: appFou 2s 1;
    }

    .column span {
        
        color: #fff;
        font-weight: bold;
        font-size: 20px;
        animation: app 3s 1;
    }

    .name {
        text-transform: uppercase;
        color: black;
        font-weight: bold;
        font-size: 25px;
        margin-top: 10px;
    }

    @keyframes appFir {
        0% {
            height: 0;
        }

        20% {
            height: 0;
        }

        100% {
            height: calc(350px / 100 * 25);
        }
    }

    @keyframes appSec {
        0% {
            height: 0;
        }

        20% {
            height: 0;
        }

        100% {
            height: calc(350px / 100 * 30);
        }
    }

    @keyframes appThi {
        0% {
            height: 0;
        }

        20% {
            height: 0;
        }

        100% {
            height: calc(350px / 100 * 40);
        }
    }

    @keyframes appFou {
        0% {
            height: 0;
        }

        20% {
            height: 0;
        }

        100% {
            height: calc(350px / 100 * 20);
        }
    }

    @keyframes app {
        0% {
            opacity: 0;
        }

        70% {
            opacity: 0;
        }

        100% {
            opacity: 1;
        }
    }
</style>
