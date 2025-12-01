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