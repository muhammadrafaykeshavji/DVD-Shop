// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Home page motion and carousel behavior.
(() => {
  const animated = document.querySelectorAll(".cv-animate");
  if (animated.length > 0) {
    const observer = new IntersectionObserver(
      (entries) => {
        entries.forEach((entry) => {
          if (entry.isIntersecting) {
            entry.target.classList.add("in-view");
            observer.unobserve(entry.target);
          }
        });
      },
      { threshold: 0.18, rootMargin: "0px 0px -30px 0px" }
    );
    animated.forEach((el) => observer.observe(el));
  }

  const boutiqueCarousels = document.querySelectorAll(".cv-boutique-carousel");
  boutiqueCarousels.forEach((el) => {
    if (window.bootstrap && window.bootstrap.Carousel) {
      window.bootstrap.Carousel.getOrCreateInstance(el, {
        interval: 4500,
        ride: "carousel",
        pause: "hover",
        touch: true,
        wrap: true
      });
    }
  });

  const dismissToast = (item) => {
    if (!item || item.classList.contains("cv-toast-item--leaving")) return;
    item.classList.add("cv-toast-item--leaving");
    item.addEventListener(
      "animationend",
      () => {
        item.remove();
        const host = document.querySelector(".cv-toast-host");
        if (host && !host.querySelector(".cv-toast-item")) host.remove();
      },
      { once: true }
    );
  };

  document.querySelectorAll(".cv-toast-item").forEach((item) => {
    const closeBtn = item.querySelector(".cv-toast-close");
    closeBtn?.addEventListener("click", () => dismissToast(item));
    window.setTimeout(() => dismissToast(item), 4500);
  });
})();

// Admin panel — sidebar toggle (persists across page loads)
(() => {
  const body = document.body;
  const root = document.documentElement;
  if (!body.classList.contains("cv-admin-body")) return;

  const toggle = document.querySelector("[data-cv-admin-sidebar-toggle]");
  const backdrop = document.querySelector("[data-cv-admin-sidebar-backdrop]");
  const desktopMq = window.matchMedia("(min-width: 992px)");
  const STORAGE_OPEN = "cvAdminSidebarOpen";
  const STORAGE_CLOSED = "cvAdminSidebarClosed";

  const isDesktop = () => desktopMq.matches;

  const isOpen = () => {
    if (isDesktop()) {
      return root.getAttribute("data-admin-sidebar") !== "closed";
    }
    return root.getAttribute("data-admin-sidebar") === "open";
  };

  const persist = (open) => {
    try {
      if (isDesktop()) {
        sessionStorage.setItem(STORAGE_CLOSED, open ? "0" : "1");
        sessionStorage.removeItem(STORAGE_OPEN);
      } else {
        sessionStorage.setItem(STORAGE_OPEN, open ? "1" : "0");
      }
    } catch {
      /* ignore */
    }
  };

  const applyState = (open, { animate = false } = {}) => {
    const state = open ? "open" : "closed";
    root.setAttribute("data-admin-sidebar", state);
    body.classList.toggle("cv-admin-sidebar-is-open", open && !isDesktop());
    body.classList.toggle("cv-admin-sidebar-is-closed", !open && isDesktop());
    body.classList.toggle("cv-admin-sidebar-animate", animate);
    toggle?.setAttribute("aria-expanded", String(open));
    if (open && !isDesktop()) {
      body.style.overflow = "hidden";
    } else {
      body.style.overflow = "";
    }
  };

  const syncFromStorage = () => {
    let open = true;
    try {
      if (isDesktop()) {
        open = sessionStorage.getItem(STORAGE_CLOSED) !== "1";
      } else {
        open = sessionStorage.getItem(STORAGE_OPEN) === "1";
      }
    } catch {
      open = isDesktop();
    }
    applyState(open, { animate: false });
  };

  toggle?.addEventListener("click", () => {
    const next = !isOpen();
    persist(next);
    applyState(next, { animate: true });
  });

  backdrop?.addEventListener("click", () => {
    persist(false);
    applyState(false, { animate: true });
  });

  document.addEventListener("keydown", (e) => {
    if (e.key === "Escape" && isOpen()) {
      persist(false);
      applyState(false, { animate: true });
    }
  });

  desktopMq.addEventListener("change", syncFromStorage);
  syncFromStorage();
})();
