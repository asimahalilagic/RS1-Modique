(function ($) {
    "use strict";

    // ---------------- CART FUNCTIONS ----------------
    function saveCart(cart) {
        localStorage.setItem("cart", JSON.stringify(cart));
        updateCartCount();
    }

    function loadCart() {
        let cart = JSON.parse(localStorage.getItem("cart")) || [];
        updateCartTotal(cart);
    }

    function updateCartTotal(cart) {
        $("tbody").empty(); // očisti tbody
        let total = 0;

        cart.forEach(item => {
            let subtotal = item.price * item.qty;
            total += subtotal;

            let newRow = `
            <tr>
                <td class="align-middle text-left">
                    <img src="${item.img}" alt="" style="width: 50px; margin-right:8px;">
                    ${item.name}
                </td>
                <td class="align-middle">${item.price.toFixed(2)} KM</td>
                <td class="align-middle">
                    <div class="input-group quantity mx-auto" style="width: 100px;">
                        <div class="input-group-btn">
                            <button class="btn btn-sm btn-primary btn-minus"><i class="fa fa-minus"></i></button>
                        </div>
                        <input type="text" class="form-control form-control-sm bg-secondary text-center" value="${item.qty}">
                        <div class="input-group-btn">
                            <button class="btn btn-sm btn-primary btn-plus"><i class="fa fa-plus"></i></button>
                        </div>
                    </div>
                </td>
                <td class="align-middle">${subtotal.toFixed(2)} KM</td>
                <td class="align-middle"><button class="btn btn-sm btn-primary"><i class="fa fa-times"></i></button></td>
            </tr>
            `;
            $("tbody").append(newRow);
        });

        $("#cart-total").text(total.toFixed(2) + " KM");
        updateCartCount();
    }

    function updateCartCount() {
        let cart = JSON.parse(localStorage.getItem("cart")) || [];
        let count = cart.reduce((sum, item) => sum + item.qty, 0);
        $("#cart-count").text(count);
    }

    // ---------------- SHOP: ADD TO CART ----------------
    $(".add-to-cart").on('click', function (e) {
        e.preventDefault();

        let productName = $(this).data("name");
        let productPrice = parseFloat($(this).data("price"));
        let productImg = $(this).data("img");

        let cart = JSON.parse(localStorage.getItem("cart")) || [];

        let found = cart.find(item => item.name === productName);
        if (found) {
            found.qty += 1;
        } else {
            cart.push({name: productName, price: productPrice, qty: 1, img: productImg});
        }

        saveCart(cart);
        updateCartTotal(cart);
    });

    // ---------------- CART QUANTITY + / - ----------------
    $("tbody").on('click', '.btn-plus, .btn-minus', function() {
        let input = $(this).closest('.quantity').find('input');
        let oldVal = parseInt(input.val());
        let cart = JSON.parse(localStorage.getItem("cart")) || [];

        let rowIndex = $(this).closest("tr").index();
        if($(this).hasClass('btn-plus')) input.val(oldVal + 1);
        else if(oldVal > 1) input.val(oldVal - 1);

        // update qty in localStorage
        cart[rowIndex].qty = parseInt(input.val());
        saveCart(cart);
        updateCartTotal(cart);
    });

    // ---------------- CART REMOVE ITEM ----------------
    $("tbody").on('click', '.fa-times', function() {
        let rowIndex = $(this).closest("tr").index();
        let cart = JSON.parse(localStorage.getItem("cart")) || [];
        cart.splice(rowIndex, 1);
        saveCart(cart);
        updateCartTotal(cart);
    });

    // ---------------- INITIALIZE ----------------
    $(document).ready(function () {
        loadCart();
    });

})(jQuery);
