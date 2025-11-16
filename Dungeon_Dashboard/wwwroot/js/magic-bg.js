const canvas = document.getElementById('fireflies');
const ctx = canvas.getContext('2d');
let w, h, fireflies = [];

function resize() {
    w = canvas.width = canvas.offsetWidth;
    h = canvas.height = canvas.offsetHeight;

    fireflies = Array.from({ length: 40 }, () => ({
        x: Math.random() * w,
        y: Math.random() * h,
        r: Math.random() * 2 + 1,
        dx: (Math.random() - 0.5) * 0.6,
        dy: (Math.random() - 0.5) * 0.6,
        flicker: Math.random() * 2 * Math.PI
    }));
}

function draw() {
    ctx.clearRect(0, 0, w, h);
    for (let f of fireflies) {
        f.x += f.dx;
        f.y += f.dy;
        if (f.x < 0 || f.x > w) f.dx *= -1;
        if (f.y < 0 || f.y > h) f.dy *= -1;
        f.flicker += 0.03;
        const alpha = 0.5 + Math.sin(f.flicker) * 0.5;
        ctx.beginPath();
        ctx.arc(f.x, f.y, f.r, 0, Math.PI * 2);
        ctx.fillStyle = `rgba(255, 255, 180, ${alpha})`;
        ctx.fill();
    }
    requestAnimationFrame(draw);
}

window.addEventListener('resize', resize);
window.addEventListener('load', resize);
resize();
draw();
