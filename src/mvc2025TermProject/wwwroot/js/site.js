// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function switchMainImage(thumbnail, filepath, alttext, description) {
    const mainImage = document.querySelector('.card-img-top img');
    if (mainImage) {
        mainImage.src = filepath;
        mainImage.alt = alttext;
        mainImage.title = description || '';

        // Optional: Add active state to clicked thumbnail
        document.querySelectorAll('.thumbnail-hover img').forEach(img => {
            img.classList.remove('border-primary');
            img.style.borderWidth = '1px';
        });
        thumbnail.classList.add('border-primary');
        thumbnail.style.borderWidth = '3px';
    }
}

function toggleIngredient(checkbox) {
    const label = checkbox.parentElement.querySelector('.ingredient-label');

    if (checkbox.checked) {
        label.style.textDecoration = 'line-through';
        label.style.opacity = '0.6';
    } else {
        label.style.textDecoration = 'none';
        label.style.opacity = '1';
    }
}