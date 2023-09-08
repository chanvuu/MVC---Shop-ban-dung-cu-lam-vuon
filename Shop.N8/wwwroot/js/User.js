var dtable;
$(document).ready(function () {
    dtable = $('#myTable').DataTable({
        ajax: {
            "url": "/Admin/User/AllUsers"
        },
        columns: [
            { data: "id" },
            { data: "email" },
            { data: "ConfirmPassword", defaultContent: "*" },
            { data: "address" },
            { data: "city" },
            { data: "state" },
            { data: "pinCode" },
            {
                data: "id",
                render: function (data) {
                    return `
                                                 <a href="/Admin/User/CreateUpdate?id=${data}"><i class="bi bi-pencil-square"></i>Edit</a>
                    <a onclick="RemoveUser('/Admin/User/Delete/${data}')"><i class="bi bi-trash"></i>Delete</a>
                    `
                }
            }
        ]
    });
});
function RemoveUser(url) {
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
                        console.log(data.id.ToString()); // In ra id ở console log

                        toastr.error(data.message);

                    }
                }
            });
        }
    })
}