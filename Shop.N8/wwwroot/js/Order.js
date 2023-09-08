var dtable;
$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("Pending")) {
        OrderTable("Pending");
    }
    else if (url.includes("Approved")) {
        OrderTable("Approved");
    }
    else if (url.includes("Cancelled")) {
        OrderTable("Cancelled");
    }
    else {
        OrderTable("all");
    }
});
function OrderTable(status) {
    dtable = $('#myTable').DataTable({
        ajax: { "url": "/Admin/Order/AllOrders?status=" + status },
        columns: [
            { data: "id" },
            { data: "name" },
            { data: "phone" },
            { data: "orderStatus" },
            { data: "orderTotal" },
            {
                data: "id",
                render: function (data) {
                    return `
                        <a href="/Admin/Order/OrderDetails?id=${data}"><i class="bi bi-pencil-square"></i>Details</a>
        <a onclick="RemoveOrder('/Admin/Order/Delete/${data}')"><i class="bi bi-trash-fill"></i>Delete</a>

                    `
                }
            }
        ]
    });

}
function RemoveOrder(url) {
        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            icon: 'Warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, delete it!'
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: url,
                    type: 'Delete',
                    success: function (data) {
                        if (data.success) {
                            dtable.ajax.reload();
                            toastr.success(data.message);
                        }
                        else {
                            toastr.error(data.message);

                        }
                    }
                });
            }
        })
    }