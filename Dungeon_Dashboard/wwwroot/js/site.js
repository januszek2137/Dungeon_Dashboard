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
    const results = [];
    const resultElement = document.getElementById("diceResult");
    resultElement.innerText = "You rolled: "; // Inicjalizacja tekstu wyniku

    let currentDice = 0; // Licznik aktualnie rzucanej kości

    function rollNextDice() {
        if (currentDice < diceCount) {
            const roll = Math.floor(Math.random() * diceType) + 1; // Rzut kostką
            results.push(roll);

            // Aktualizacja wyniku w trakcie rzutu
            resultElement.innerText = `You rolled: ${results.join(", ")}`;

            currentDice++;

            // Jeśli rzucamy więcej niż jedną kością, dodajemy opóźnienie
                setTimeout(rollNextDice, 20); // Opóźnienie przed następnym rzutem
        } else {
            // Dodanie sumy po zakończeniu rzutów
            const total = results.reduce((a, b) => a + b, 0);
            resultElement.innerText += ` (Total: ${total})`;
        }
    }

    rollNextDice(); // Rozpoczęcie sekwencji rzutów
}





