$(function () {
    $.fn.editable.defaults.mode = 'inline';
});

$("#countCustomers").click(function () {
    $.ajax({
        url: '/api/customers/count',
        method: 'GET'
    }).done(function (result) {
        console.log(result);
        $('#status').text("number of customers in list: " + result);
        $('#status').append("<hr />");
    });
});

$("#addForm button").click(function () {
    console.log(this);
    if (this.id == "customerButton") {
        $.ajax({
            url: '/api/customers',
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
                $("#getAllCustomers").click();
                console.log("Success!", result);
                $('#status').text("new customer created");
                $('#status').append("<hr />");
            })

            .fail(function (xhr, status, error) {
                let errorMessages = xhr.responseJSON;
                let concatinatedErrorMessage = "";

                $.each(errorMessages, function (index, item) {
                    concatinatedErrorMessage += `${item[0]} `;
                });
                $("#status").text(concatinatedErrorMessage);
                $('#status').append("<hr />");
                console.log("Error", xhr, status, error);
            });
    }
    else if (this.id == "addressButton") {
        $.ajax({
            url: '/api/address',
            method: 'POST',
            data: {
                "StreetName": $("#addForm [name=StreetName]").val(),
                "StreetNumber": $("#addForm [name=StreetNumber]").val(),
                "PostalCode": $("#addForm [name=PostalCode]").val(),
                "Area": $("#addForm [name=Area]").val()
            }
        })
            .done(function (result) {
                $("#getAllAddresses").click();
                console.log("Success!", result);
                $('#status').text("new address created");
                $('#status').append("<hr />");
            })

            .fail(function (xhr, status, error) {
                let errorMessages = xhr.responseJSON;
                let concatinatedErrorMessage = "";

                $.each(errorMessages, function (index, item) {
                    concatinatedErrorMessage += `${item[0]} `;
                });
                $("#status").text(concatinatedErrorMessage);
                $('#status').append("<hr />");
                console.log("Error", xhr, status, error);
            });
    }
    else console.log("error");
});

$("#getOneCustomer").click(function () {
    let idNumber = $("#idNumber").val();
    console.log(idNumber);
    $.ajax({
        url: `/api/customers/${idNumber}`,
        method: 'GET'
    })
        .done(function (result) {
            if (idNumber == "") {
                $("#status").text(`Please enter a number`);
            }
            else if (result.length == 1) {
                $("#status").text(`A customer with the Id ${idNumber} was not found`);
            }
            else {
                $("#status").text(`${result.id}. ${result.firstName} ${result.lastName} - ${result.gender} - ${result.email} - ${result.age} years old`);
            }

            $('#status').append("<hr />");
        })

        .fail(function (xhr, status, error) {
            $("#status").text(xhr.responseText);
            $('#status').append("<hr />");
        });
});


$("#getAllCustomers").click(function () {
    $.ajax({
        url: '/api/customers',
        method: 'GET'
    })
        .done(function (result) {
            let generatedResult = '<table class="table table-sm table-dark table-striped"><thead><tr>'
                + '<th scope= "col">#</th>'
                + '<th scope= "col">First Name</th>'
                + '<th scope= "col">Last Name</th>'
                + '<th scope= "col">Gender</th>'
                + '<th scope= "col">Email</th>'
                + '<th scope= "col">Age</th>'
                + '<th scope= "col">Adress</th>'
                + '<th scope= "col">Delete</th>'
                + '</tr ></thead ><tbody>';
            if (result.length == 0) {
                generatedResult += '<h2>No customers</h2>'
            }

            else {
                $.each(result, function (index, item) {
                    console.log(item);
                    generatedResult += '<tr id="customerNumber' + (index + 1) + '">' +
                        `<th scope="row">${item.id}</th>` +
                        `<td><a href="#" class="edit" data-name="firstName" data-type="text" data-pk="${item.id}" data-title="Enter First Name">${item.firstName}</a></td>` +
                        `<td><a href="#" class="edit" data-name="lastName" data-type="text" data-pk="${item.id}" data-title="Enter Last Name">${item.lastName}</a></td>` +
                        `<td><a href="#" class="edit" data-name="gender" data-type="text" data-pk="${item.id}" data-title="Enter Gender">${item.gender}</a></td>` +
                        `<td><a href="#" class="edit" data-name="email" data-type="text" data-pk="${item.id}" data-title="Enter Email">${item.email}</a></td>` +
                        `<td><a href="#" class="edit" data-name="age" data-type="text" data-pk="${item.id}" data-title="Enter Age">${item.age}</a></td>` +
                        `<td><div data-name="dateCreated" data-type="text" data-pk="${item.id}">${item.dateCreated.substring(0, 10)}</div></td>`;
                    if (item.dateEdited != null) generatedResult += `<td><div data-name="dateEdited" data-type="text" data-pk="${item.id}">${item.dateEdited.substring(0, 10)}</div></td>`; 
                    else generatedResult += `<td><div data-name="dateEdited" data-type="text" data-pk="${item.id}">Never edited</div></td>`; 
                    generatedResult += `<td><a href="#" class="addressModule" id="${item.id}" data-title="Address"><button class="btn btn-info">A</button></a></td>` +
                        `<td><a href="#" class="delete" id="${item.id}" data-title="Delete"><button class="btn btn-danger">X</button></a></td>` +
                        '</tr>';
                });
            }

            generatedResult += "</tbody ></table >";

            $("#dataTable").html(generatedResult);

            $(".edit").editable({
                type: 'text',
                url: `/api/customers/${this.id}`,
                fail: function (response) {
                    console.log(response);
                }
            });

            $(".delete").click(function () {
                let deleteId = this.id;
                console.log(deleteId);
                $.ajax({
                    url: '/api/customers/' + deleteId,
                    method: 'DELETE'

                }).done(function () {
                    $("#getAllCustomers").click();
                    $("#status").text("customer deleted");
                    $('#status').append("<hr />");
                });
            });
            let addressesForACustomer;

            $(".addressModule").click(function () {
                let idForCustomer = this.id;

                console.log(this.id);
                $.ajax({
                    url: `/api/customers/${idForCustomer}/address`,
                    method: 'GET'

                }).done(function (result) {
                    addressesForACustomer = result;
                    let modalContent = '<div class="modal-dialog" role="document">'
                        + '<div class="modal-content">'
                        + '<div class="modal-header">'
                        + `<h5 class="modal-title customer" id="addressModalLabel" value="${idForCustomer}">Addresses</h5>`
                        + '<button type="button" class="close" data-dismiss="modal" aria-label="Close">'
                        + '<span aria-hidden="true">&times;</span>'
                        + '</button>'
                        + '</div>'
                        + '<div class="modal-body">'
                        + '<table class="table">'
                        + '<tbody>';

                    if (result.length === 0) {
                        modalContent += '<h2>No addresses</h2>';
                    }
                    else {
                        modalContent += '<thead><tr>'
                            + '<th scope="col">#</th>'
                            + '<th scope="col">Street</th>'
                            + '<th scope="col">Number</th> '
                            + '<th scope="col">Postcode</th> '
                            + '<th scope="col">Area</th> '
                            + '<th scope="col">Delete</th> '
                            + '</tr></thead>';

                        $.each(result, function (index, item) {
                            modalContent += `<tr><th scope="row">${item.id}</th >`
                                + `<td><span href="#" class="editAddress" data-name="streetName" data-type="text" data-pk="${item.id}" data-title="Enter Street Name">${item.streetName}</span></td>`
                                + `<td><span href="#" class="editAddress" data-name="streetNumber" data-type="text" data-pk="${item.id}" data-title="Enter Street Number">${item.streetNumber}</span></td>`
                                + `<td><span href="#" class="editAddress" data-name="postalCode" data-type="text" data-pk="${item.id}" data-title="Enter Postal Code">${item.postalCode}</span></td>`
                                + `<td><span href="#" class="editAddress" data-name="area" data-type="text" data-pk="${item.id}" data-title="Enter Area Name">${item.area}</span></td>`
                                + `<td class="addressDelete" id=${item.id}><button class="btn btn-danger">X</button></td>`
                                + '</td ></tr >';
                        });
                    }

                    modalContent += '</tbody >'
                        + '</table >'
                        + '</div >'
                        + '<div class="modal-footer">'
                        + '<div class="test">';
                    
                    modalContent += '</div>'
                        + '<button type="button" class="btn btn-primary newAddress">Add New</button>'
                        + '</div></div></div>';
                    $("#addressModal").html(modalContent);
                    $("#addressModal").modal(show = true);
                    
                    AddressFunctions(idForCustomer, addressesForACustomer);


                    $(".newAddress").click(function () {
                        console.log(valueOfSelectedAddress);
                        $.ajax({
                            url: `/api/customers/${idForCustomer}/address/${valueOfSelectedAddress}`,
                            method: 'POST',
                            data: {
                                "custId": idForCustomer,
                                "AdressId": valueOfSelectedAddress
                            }
                        })
                            .done(function (result) {
                                $("#status").text(result);
                                $('#status').append("<hr />");
                            })
                            .fail(function (result) {
                                $("#status").text(result);
                                $('#status').append("<hr />");
                            })
                    });
                });
            });
        })

        .fail(function (xhr, status, error) {
            s
            $("#status").text(xhr.responseText);
            $('#status').append("<hr />");
        });
});

let valueOfSelectedAddress;

function AddressFunctions(customerId, addressesForACustomer) {

    valueOfSelectedAddress = null;

    getAddressSelectMenu(customerId, addressesForACustomer);

    $(".addressDelete").click(function () {
        let idOfAddress = this.id;
        $.ajax({
            url: `api/customers/${customerId}/address/${idOfAddress}`,
            method: 'DELETE'
        })
            .done(function (result) {
                $("#status").text(result);
                $('#status').append("<hr />");
            });
    });

    $(".editAddress").editable({
        type: 'text',
        url: `/api/address/${this.id}`,
        success: function (response) {
            $("#status").html(response)
        },
        fail: function (response) {
            $("#status").html(response);
        }
    });
}

function getAddressSelectMenu(customerId, addressesForACustomer) {
    console.log(addressesForACustomer);
    $.ajax({
        url: '/api/address',
        method: 'GET'
    })
        .done(function (result) {
            let test = '<select id="selectedAddress" onchange="changeValue(this)">>'
                + '<option>select one</option>';
            $.each(result, function (index, item) {
                var noPreviousRelation = false;
                $.each(addressesForACustomer, function (index, alreadyOnCustomer) {
                    if (alreadyOnCustomer.id == item.id) {
                        noPreviousRelation = true; return;
                    }
                });
                if (!noPreviousRelation) test += `<option value= "${item.id}">${item.streetName} ${item.streetNumber}</option >`
            });
            test += '</select>';
            $(".test").html(test);
        });
}

function changeValue(selectObject) {
    valueOfSelectedAddress = selectObject.value;
    console.log(valueOfSelectedAddress);
}

$("#seedCustomers").click(function () {
    $.ajax({
        url: '/api/customers/seed',
        method: 'GET'
    })
        .done(function (result) {
            $("#getAllCustomers").click();
            $("#status").text(result);
            $('#status').append("<hr />");
        })

        .fail(function (xhr, status, error) {
            $("#status").text(xhr.responseText);
            $('#status').append("<hr />");
        });
});

$("#getAllAddresses").click(function () {
    $.ajax({
        url: '/api/address',
        method: 'GET'
    })
        .done(function (result) {
            let generatedResult = '<table class="table table-sm table-dark table-striped"><thead><tr>'
                + '<th scope= "col">#</th>'
                + '<th scope= "col">Street Name</th>'
                + '<th scope= "col">Street Number</th>'
                + '<th scope= "col">Postal Code</th>'
                + '<th scope= "col">Area</th>'
                + '</tr ></thead ><tbody>';
            if (result.length == 0) {
                generatedResult += '<h2>No Addresses</h2>'
            }

            else {
                $.each(result, function (index, item) {
                    console.log(item);
                    generatedResult += '<tr id="customerNumber' + (index + 1) + '">' +
                        `<th scope="row">${item.id}</th>` +
                        `<td><a href="#" class="edit" data-name="streetName" data-type="text" data-pk="${item.id}" data-title="Enter Street Name">${item.streetName}</a></td>` +
                        `<td><a href="#" class="edit" data-name="streetNumber" data-type="text" data-pk="${item.id}" data-title="Enter Street Number">${item.streetNumber}</a></td>` +
                        `<td><a href="#" class="edit" data-name="postalCode" data-type="text" data-pk="${item.id}" data-title="Enter Postal Code">${item.postalCode}</a></td>` +
                        `<td><a href="#" class="edit" data-name="area" data-type="text" data-pk="${item.id}" data-title="Enter Area">${item.area}</a></td>` +
                        `<td><a href="#" class="delete" id="${item.id}" data-title="Delete"><button class="btn btn-danger">X</button></a></td>` +
                        '</tr>';
                });
            }

            generatedResult += "</tbody ></table >";

            $("#dataTable").html(generatedResult);

            $(".edit").editable({
                type: 'text',
                url: `/api/address/${this.id}`,
                fail: function (response) {
                    console.log(response);
                }
            });

            $(".delete").click(function () {
                let deleteId = this.id;
                console.log(deleteId);
                $.ajax({
                    url: `/api/address/${deleteId}`,
                    method: 'DELETE'

                }).done(function () {
                    $("#getAllAddresses").click();
                    $("#status").text("address deleted");
                    $('#status').append("<hr />");
                });
            });
        })

        .fail(function (xhr, status, error) {
            s
            $("#status").text(xhr.responseText);
            $('#status').append("<hr />");
        });
});