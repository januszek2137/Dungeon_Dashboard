document.getElementById("toggle-dashboard").addEventListener("click", function () {
    var sidebar = document.getElementById("sidebar");
    var contentWrapper = document.getElementById("content-wrapper");
    sidebar.classList.toggle("active");
    contentWrapper.classList.toggle("active");

    if (this.classList.contains("rotate-180")) {
        this.classList.remove("rotate-180");
        this.classList.add("rotate-360");
    } else if (this.classList.contains("rotate-360")) {
        this.classList.remove("rotate-360");
        this.classList.add("rotate-180");
    } else {
        this.classList.add("rotate-180");
    }
});

function updateSliderValue(sliderId, valueId) {
    const slider = document.getElementById(sliderId);
    const value = document.getElementById(valueId);

    value.textContent = slider.value;

    slider.addEventListener('input', () => {
        value.textContent = slider.value;
    });
}
updateSliderValue('sliderStrength', 'sliderValueStrength');
updateSliderValue('sliderDexterity', 'sliderValueDexterity');
updateSliderValue('sliderConstitution', 'sliderValueConstitution');
updateSliderValue('sliderIntelligence', 'sliderValueIntelligence');
updateSliderValue('sliderWisdom', 'sliderValueWisdom');
updateSliderValue('sliderCharisma', 'sliderValueCharisma');

function rollDice() {
    const diceType = parseInt(document.getElementById("diceType").value);
    const diceCount = parseInt(document.getElementById("diceCount").value);
    const diceContainer = document.getElementById("diceContainer");
    const resultElement = document.getElementById("diceResult");

    diceContainer.innerHTML = '';
    resultElement.innerText = '';

    if (diceCount > 50) {
        errorMessage.innerText = "You cannot roll more than 50 dice at once";
        return;
    }

    const results = [];
    let diceFinished = 0;

    errorMessage.innerText = "";

    for (let i = 0; i < diceCount; i++) {
        const die = document.createElement('div');
        die.className = 'die';

        switch (diceType) {
            case 4:
                die.classList.add('d4');
                break;
            case 6:
                die.classList.add('d6');
                break;
            case 8:
                die.classList.add('d8');
                break;
            case 10:
                die.classList.add('d10');
                break;
            case 12:
                die.classList.add('d12');
                break;
            case 20:
                die.classList.add('d20');
                break;
            case 100:
                die.classList.add('d100');
                break;
            default:
                break;
        }

        die.innerHTML = '<span class="die-value"></span>';

        diceContainer.appendChild(die);

        let animationDuration = 1000 + Math.random() * 1000;
        let startTime = Date.now();

        const interval = setInterval(function () {
            let elapsed = Date.now() - startTime;

            let randomFace = Math.floor(Math.random() * diceType) + 1;
            die.querySelector('.die-value').innerText = randomFace;

            if (elapsed >= animationDuration) {
                const finalValue = Math.floor(Math.random() * diceType) + 1;
                die.querySelector('.die-value').innerText = finalValue;
                results.push(finalValue);
                diceFinished++;
                clearInterval(interval);

                if (diceFinished === diceCount) {
                    const total = results.reduce((a, b) => a + b, 0);
                    resultElement.innerText = `Total: ${total}`;
                }
            }
        }, 50);
    }
}