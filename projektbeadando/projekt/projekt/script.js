/* ================= ADMIN ================= */
let isAdmin = false;

function toggleAdmin() {
    isAdmin = !isAdmin;
    document.getElementById("adminPanel").classList.toggle("active");
    renderRecipes();
}

/* ================= UI ================= */
function toggleDarkMode() {
    document.body.classList.toggle("dark");
}

function toggleMenu() {
    document.getElementById("navLinks").classList.toggle("show");
}

/* ================= ZÁSZLÓK ================= */
const countryFlags = {
    "Magyarország": "🇭🇺",
    "Olaszország": "🇮🇹",
    "Mexikó": "🇲🇽",
    "Japán": "🇯🇵",
    "India": "🇮🇳",
    "USA": "🇺🇸",
    "Kína": "🇨🇳"
};

/* ================= DOM ================= */
const recipesGrid = document.getElementById("recipesGrid");
const countryFilter = document.getElementById("countryFilter");

const nameInput = document.getElementById("recipeName");
const descInput = document.getElementById("recipeDesc");
const timeInput = document.getElementById("recipeTime");
const countryInput = document.getElementById("recipeCountry");
const editIndexInput = document.getElementById("editIndex");

const formTitle = document.getElementById("formTitle");
const submitBtn = document.getElementById("submitBtn");
const cancelBtn = document.getElementById("cancelBtn");

/* ================= ALAP RECEPTEK ================= */
const defaultRecipes = [
    // Magyar (5)
    { name: "Gulyásleves", desc: "Hagyományos magyar leves.", time: "90 perc", country: "Magyarország" },
    { name: "Csirkepaprikás", desc: "Paprikás csirke.", time: "60 perc", country: "Magyarország" },
    { name: "Töltött káposzta", desc: "Savanyú káposztás étel.", time: "120 perc", country: "Magyarország" },
    { name: "Lángos", desc: "Fokhagymás lángos.", time: "30 perc", country: "Magyarország" },
    { name: "Hortobágyi palacsinta", desc: "Húsos palacsinta.", time: "45 perc", country: "Magyarország" },

    // Olasz (5)
    { name: "Pizza Margherita", desc: "Paradicsomos pizza.", time: "30 perc", country: "Olaszország" },
    { name: "Carbonara", desc: "Krémes tészta.", time: "25 perc", country: "Olaszország" },
    { name: "Lasagne", desc: "Réteges tészta.", time: "60 perc", country: "Olaszország" },
    { name: "Risotto", desc: "Krémes rizottó.", time: "40 perc", country: "Olaszország" },
    { name: "Tiramisu", desc: "Kávés desszert.", time: "35 perc", country: "Olaszország" },

    // Mexikói (5)
    { name: "Tacos", desc: "Töltött tortilla.", time: "25 perc", country: "Mexikó" },
    { name: "Burrito", desc: "Tekercselt tortilla.", time: "30 perc", country: "Mexikó" },
    { name: "Quesadilla", desc: "Sajtos tortilla.", time: "20 perc", country: "Mexikó" },
    { name: "Nachos", desc: "Sajtos chips.", time: "15 perc", country: "Mexikó" },
    { name: "Chili con carne", desc: "Fűszeres hús.", time: "50 perc", country: "Mexikó" },

    // Japán (5)
    { name: "Sushi", desc: "Rizses hal.", time: "50 perc", country: "Japán" },
    { name: "Ramen", desc: "Tésztaleves.", time: "45 perc", country: "Japán" },
    { name: "Tempura", desc: "Bundázott zöldség.", time: "30 perc", country: "Japán" },
    { name: "Onigiri", desc: "Rizsgolyó.", time: "15 perc", country: "Japán" },
    { name: "Teriyaki csirke", desc: "Édes-sós csirke.", time: "35 perc", country: "Japán" },

    // Indiai (5)
    { name: "Butter Chicken", desc: "Krémes csirke.", time: "45 perc", country: "India" },
    { name: "Biryani", desc: "Fűszeres rizs.", time: "60 perc", country: "India" },
    { name: "Naan", desc: "Indiai kenyér.", time: "20 perc", country: "India" },
    { name: "Tikka Masala", desc: "Paradicsomos csirke.", time: "40 perc", country: "India" },
    { name: "Samosa", desc: "Töltött tészta.", time: "30 perc", country: "India" },

    // USA (5)
    { name: "Hamburger", desc: "Marhahúsos burger.", time: "35 perc", country: "USA" },
    { name: "Hot dog", desc: "Virslis szendvics.", time: "15 perc", country: "USA" },
    { name: "BBQ ribs", desc: "Sült borda.", time: "90 perc", country: "USA" },
    { name: "Mac and Cheese", desc: "Sajtos tészta.", time: "25 perc", country: "USA" },
    { name: "Pancake", desc: "Amerikai palacsinta.", time: "20 perc", country: "USA" },

    // Kína (5)
    { name: "Kung Pao csirke", desc: "Csípős mogyorós csirke.", time: "30 perc", country: "Kína" },
    { name: "Édes-savanyú csirke", desc: "Klasszikus kínai étel.", time: "35 perc", country: "Kína" },
    { name: "Tavaszi tekercs", desc: "Ropogós előétel.", time: "25 perc", country: "Kína" },
    { name: "Sült rizs", desc: "Wokban sült rizs.", time: "20 perc", country: "Kína" },
    { name: "Wonton leves", desc: "Töltött tésztaleves.", time: "40 perc", country: "Kína" }
];

/* ================= BETÖLTÉS ================= */
let recipes;

if (!localStorage.getItem("recipes_v2")) {
    recipes = defaultRecipes;
    localStorage.setItem("recipes_v2", JSON.stringify(recipes));
} else {
    recipes = JSON.parse(localStorage.getItem("recipes_v2"));
}

/* ================= SZŰRŐ ================= */
function populateCountryFilter() {
    const countries = [...new Set(recipes.map(r => r.country))];

    countryFilter.innerHTML = `<option value="all">🌍 Összes</option>`;

    countries.forEach(c => {
        const opt = document.createElement("option");
        opt.value = c;
        opt.textContent = `${countryFlags[c]} ${c}`;
        countryFilter.appendChild(opt);
    });
}

/* ================= MEGJELENÍTÉS ================= */
function renderRecipes() {
    recipesGrid.innerHTML = "";
    const selected = countryFilter.value;

    recipes.forEach((r, i) => {
        if (selected !== "all" && r.country !== selected) return;

        recipesGrid.innerHTML += `
            <div class="recipe-card">
                <h3>${countryFlags[r.country]} ${r.name}</h3>
                <p>${r.desc}</p>
                <span>⏱️ ${r.time}</span><br>
                <span>🌍 ${r.country}</span>
                <div style="margin-top:10px;">
                    <button onclick="editRecipe(${i})">✏️ Szerkesztés</button>
                    ${isAdmin ? `<button onclick="deleteRecipe(${i})">🗑️ Törlés</button>` : ""}
                </div>
            </div>
        `;
    });
}

function filterRecipes() {
    renderRecipes();
}

/* ================= CRUD ================= */
function handleSubmit(e) {
    e.preventDefault();

    const recipe = {
        name: nameInput.value,
        desc: descInput.value,
        time: timeInput.value,
        country: countryInput.value
    };

    const idx = editIndexInput.value;
    idx === "" ? recipes.push(recipe) : recipes[idx] = recipe;

    localStorage.setItem("recipes_v2", JSON.stringify(recipes));
    resetForm();
    populateCountryFilter();
    renderRecipes();
}

function editRecipe(i) {
    const r = recipes[i];
    nameInput.value = r.name;
    descInput.value = r.desc;
    timeInput.value = r.time;
    countryInput.value = r.country;
    editIndexInput.value = i;

    formTitle.innerText = "✏️ Recept szerkesztése";
    submitBtn.innerText = "Mentés";
    cancelBtn.style.display = "inline";
}

function cancelEdit() {
    resetForm();
}

function resetForm() {
    nameInput.value = "";
    descInput.value = "";
    timeInput.value = "";
    countryInput.value = "";
    editIndexInput.value = "";

    formTitle.innerText = "📤 Recept feltöltése";
    submitBtn.innerText = "Feltöltés";
    cancelBtn.style.display = "none";
}

function deleteRecipe(i) {
    if (!isAdmin) return;
    if (confirm("Biztosan törlöd?")) {
        recipes.splice(i, 1);
        localStorage.setItem("recipes_v2", JSON.stringify(recipes));
        populateCountryFilter();
        renderRecipes();
    }
}

/* ================= INIT ================= */
populateCountryFilter();
renderRecipes();
