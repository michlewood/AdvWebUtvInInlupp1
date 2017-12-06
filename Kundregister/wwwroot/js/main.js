﻿$(function () {
    $.fn.editable.defaults.mode = 'inline';
});

$("#countCustomers").click(function () {
    $.ajax({
        url: '/api/customers/countcustomers',
        method: 'GET'
    }).done(function (result) {
        console.log(result);
        $('#status').text("number of customers in list: " + result);
        $('#status').append("<hr />");
    });
});

$("#addForm button").click(function () {

    $.ajax({
        url: '/api/Customers/addnewcustomer',
        method: 'POST',
        data: {
            "FirstName": $("#addForm [name=FirstName]").val(),
            "LastName": $("#addForm [name=LastName]").val(),
            "Age": $("#addForm [name=Age]").val(),
            "Gender": $("#addForm [name=Gender]").val(),
            "Email": $("#addForm [name=Email]").val()
        }
    })
        .done(function (result) {

            $("#getAll").click();
            console.log("Success!", result);
            $('#status').text("new customer created");
            $('#status').append("<hr />");
        })

        .fail(function (xhr, status, error) {
            let errorMessages = xhr.responseJSON;
            let concatinatedErrorMessage = "";

            $.each(errorMessages, function (index, item) {
                concatinatedErrorMessage += item[0] + " ";
            });
            $("#status").text(concatinatedErrorMessage);
            $('#status').append("<hr />");
            console.log("Error", xhr, status, error);
        });
});

$("#getOne").click(function () {
    let idNumber = $("#idNumber").val();
    console.log(idNumber);
    $.ajax({
        url: '/api/customers/getusingid',
        method: 'GET',
        data: { id: idNumber }
    })
        .done(function (result) {
            $("#status").text(result);
            $('#status').append("<hr />");
        })

        .fail(function (xhr, status, error) {
            $("#status").text(xhr.responseText);
            $('#status').append("<hr />");
        });
});


$("#getAll").click(function () {
    $.ajax({
        url: '/api/customers/getallcustomers',
        method: 'GET'
    })
        .done(function (result) {

            let generatedResult = '<table class="table table-sm table-dark table-striped"><thead><tr><th scope="col">#</th><th scope="col">First Name</th><th scope="col">Last Name</th><th scope="col">Gender</th><th scope="col">Email</th><th scope="col">Age</th><th scope="col">Adress</th><th scope="col">Delete</th></tr></thead><tbody>';

            $.each(result, function (index, item) {
                console.log(item);
                generatedResult += '<tr id="customerNumber' + (index + 1) + '">' +
                    '<th scope="row">' + item.id + '</th>' +
                    '<td><a href="#" class="edit" data-name="firstName" data-type="text" data-pk="' + item.id + '" data-title="Enter First Name">' + item.firstName + '</a></td>' +
                    '<td><a href="#" class="edit" data-name="lastName" data-type="text" data-pk="' + item.id + '" data-title="Enter Last Name">' + item.lastName + '</a></td>' +
                    '<td><a href="#" class="edit" data-name="gender" data-type="text" data-pk="' + item.id + '" data-title="Enter Gender">' + item.gender + '</a></td>' +
                    '<td><a href="#" class="edit" data-name="email" data-type="text" data-pk="' + item.id + '" data-title="Enter Email">' + item.email + '</a></td>' +
                    '<td><a href="#" class="edit" data-name="age" data-type="text" data-pk="' + item.id + '" data-title="Enter Age">' + item.age + '</a></td>' +
                    '<td><a href="#" class="address" id="' + item.id + '" data-title="Address"><button class="btn btn-info">A</button></a></td>' +
                    '<td><a href="#" class="delete" id="' + item.id + '" data-title="Delete"><button class="btn btn-danger">X</button></a></td>' +
                    '</tr>';
            });

            generatedResult += "</tbody ></table >";

            $("#dataTable").html(generatedResult);

            $(".edit").editable({
                type: 'text',
                url: '/api/customers/editcustomer',
                fail: function (response) {
                    console.log(response);
                }
            });

            $(".delete").click(function () {
                let deleteId = this.id;
                console.log(deleteId);
                $.ajax({
                    url: '/api/customers/deletecustomer',
                    method: 'POST',
                    data: { id: deleteId }

                }).done(function () {
                    $("#getAll").click();
                    $("#status").text("customer deleted");
                    $('#status').append("<hr />");
                });
            });

            $(".address").click(function () {
                let idForCustomer = this.id;

                console.log(this.id);
                $.ajax({
                    url: '/api/customers/showcustomeraddresses',
                    method: 'GET',
                    data: { id: idForCustomer }

                }).done(function (result) {
                    let modalContent = '<div class="modal-dialog" role="document">'
                        + '<div class="modal-content">'
                        + '<div class="modal-header">'
                        + '<h5 class="modal-title customer" id="addressModalLabel" value="' + idForCustomer + '">Addresses</h5>'
                        + '<button type="button" class="close" data-dismiss="modal" aria-label="Close">'
                        + '<span aria-hidden="true">&times;</span>'
                        + '</button>'
                        + '</div>'
                        + '<div class="modal-body">'
                        + '<table class="table">'
                        + '<tbody>'
                        + '<thead><tr>'
                        + '<th scope="col">#</th>'
                        + '<th scope="col">Street</th>'
                        + '<th scope="col">Number</th> '
                        + '<th scope="col">Postcode</th> '
                        + '<th scope="col">Area</th> '
                        + '<th scope="col">Delete</th> '
                        + '</tr></thead> ';

                    $.each(result, function (index, item) {
                        modalContent += '<tr><th scope="row">' + item.id + '</th><td>' + item.streetName + '</td><td>' + item.streetNumber + '</td><td>' + item.postalCode + '</td><td>' + item.area + '</td>'
                            + '<td><a href="#" class="addressDelete" id="' + item.id + '" data-title="Delete"><button class="btn btn-danger">X</button></a></td></tr>';
                    });

                    modalContent += '</tbody >'
                        + '</table >'
                        + '</div >'
                        + '<div class="modal-footer">'
                        + '<button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>'
                        + '<button type="button" class="btn btn-primary NewAddress">Add New</button>'
                        + '</div></div></div>';
                    $("#addressModal").html(modalContent);
                    $("#addressModal").modal(show = true);

                    AddressFunctions();
                });

            });


        })

        .fail(function (xhr, status, error) {
            $("#status").text(xhr.responseText);
            $('#status').append("<hr />");
        });
});

function AddressFunctions() {
    $(".NewAddress").click(function (id) {
        let idOfCustomer = parseInt(document.body.getElementsByClassName("customer")[0].getAttribute("value"));
        console.log(idOfCustomer);
        $.ajax({
            url: 'api/customers/addnewaddress',
            method: 'POST',
            data: { id: idOfCustomer }
        })
            .done(function (result) {
                $("#status").text(result);
                $('#status').append("<hr />");
            });
    });

    $(".addressDelete").click(function (id) {
        let idOfCustomer = parseInt(document.body.getElementsByClassName("customer")[0].getAttribute("value"));
        let idOfAddress = this.id;
        console.log(idOfAddress);
        $.ajax({
            url: 'api/customers/deleteaddress',
            method: 'POST',
            data: { custId: idOfCustomer, addressId: idOfAddress }
        })
            .done(function (result) {
                $("#status").text(result);
                $('#status').append("<hr />");
            });
    });
}

$("#seedCustomers").click(function () {
    $.ajax({
        url: '/api/customers/seedcustomers',
        method: 'GET'
    })
        .done(function (result) {
            $("#getAll").click();
            $("#status").text(result);
            $('#status').append("<hr />");
        })

        .fail(function (xhr, status, error) {
            $("#status").text(xhr.responseText);
            $('#status').append("<hr />");
        });
});