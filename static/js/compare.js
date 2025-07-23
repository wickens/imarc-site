document.addEventListener("DOMContentLoaded", () => {
  document.querySelectorAll(".compare").forEach(wrapper => {
    const before = wrapper.querySelector("img[data-compare='before']");
    const after  = wrapper.querySelector("img[data-compare='after']");
    if (!before || !after) return;

    // Build overlay structure
    const afterWrap = document.createElement("div");
    afterWrap.className = "compare__after";
    afterWrap.appendChild(after);

    const handle = document.createElement("div");
    handle.className = "compare__handle";

    const range = document.createElement("input");
    range.type = "range";
    range.min = 0;
    range.max = 100;
    range.value = 50;
    range.className = "compare__range";
    range.setAttribute("aria-label", "Image comparison slider");

    const labels = document.createElement("div");
    labels.className = "compare__labels";
    const beforeLbl = wrapper.dataset.beforeLabel || "Before";
    const afterLbl  = wrapper.dataset.afterLabel  || "After";
    labels.innerHTML = `<span>${beforeLbl}</span><span>${afterLbl}</span>`;

    // Clear and rebuild
    wrapper.innerHTML = "";
    wrapper.appendChild(before);
    wrapper.appendChild(afterWrap);
    wrapper.appendChild(handle);
    wrapper.appendChild(range);
    wrapper.appendChild(labels);

    // Ensure wrapper height matches first image aspect ratio
    const setHeight = () => {
      const w = wrapper.clientWidth;
      const ratio = before.naturalHeight / before.naturalWidth;
      wrapper.style.height = (w * ratio) + "px";
    };

    const sync = (pct) => {
      const right = 100 - pct;
      afterWrap.style.clipPath = `inset(0 ${right}% 0 0)`;
      handle.style.left = pct + "%";
      range.value = pct;
    };

    // Events
    const onPointer = (e) => {
      const rect = wrapper.getBoundingClientRect();
      const x = (e.touches ? e.touches[0].clientX : e.clientX) - rect.left;
      let pct = (x / rect.width) * 100;
      pct = Math.max(0, Math.min(100, pct));
      sync(pct);
    };

    range.addEventListener("input", () => sync(range.value));

    wrapper.addEventListener("pointerdown", e => {
      onPointer(e);
      wrapper.setPointerCapture(e.pointerId);
      wrapper.addEventListener("pointermove", onPointer);
    });
    wrapper.addEventListener("pointerup", e => {
      wrapper.releasePointerCapture(e.pointerId);
      wrapper.removeEventListener("pointermove", onPointer);
    });

    // Touch fallback
    wrapper.addEventListener("touchstart", onPointer, { passive: true });
    wrapper.addEventListener("touchmove", onPointer,  { passive: true });

    // Wait for images to load to set height correctly
    const maybeInit = () => {
      if (before.complete && after.complete) {
        setHeight();
        sync(50);
      }
    };
    before.addEventListener("load", maybeInit);
    after.addEventListener("load", maybeInit);
    // If already loaded
    maybeInit();
    window.addEventListener("resize", setHeight);
  });
});