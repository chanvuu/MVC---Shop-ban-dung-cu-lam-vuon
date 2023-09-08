var dtable;
$(document).ready(function () {
    dtable = $('#myTable').DataTable({
        ajax: { "url": "/Admin/Product/AllProducts" },
        columns: [
            { data: "id" },
            { data: "productName" },
            { data: "description" },
            {
                data: "price",
                render: function (data, type, row) {
                    return numeral(data).format('0,0.[000] VND');
                }
            },
            { data: "quatity" },
            { data: "category.categoryName" },
            {
                data: "id",
                render: function (data) {
                    return `
                                                 <a href="/Admin/Product/CreateUpdate?id=${data}"><i class="bi bi-pencil-square"></i>Edit</a>
                    <a onclick="RemoveProduct('/Admin/Product/Delete/${data}')"><i class="bi bi-trash"></i>Delete</a>
                    `
                }
            }
        ]
    });
});

function RemoveProduct(url) {
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
