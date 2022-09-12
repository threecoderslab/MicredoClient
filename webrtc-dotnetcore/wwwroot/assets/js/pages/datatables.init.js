$(document).ready(function () {
    $("#datatable").DataTable(),
        $("#datatable-buttons")
            .DataTable({
                lengthChange: !1,
                buttons: [
                    'colvis',
                    {
                        extend: 'copyHtml5',
                        text: 'Copy curent table',
                        exportOptions: {
                            columns: ':visible'
                        }
                    },
                    {
                        extend: 'excelHtml5',
                        text: 'Export Excel',
                        exportOptions: {
                            columns: ':visible'
                        }
                    },
                    {
                        extend: 'pdfHtml5',
                        text: 'Export PDF',
                        exportOptions: {
                            columns: ':visible'
                        }
                    },
                    {
                        extend: 'print',
                        text: 'Print current page',
                        exportOptions: {
                            columns: ':visible'
                        }
                    },
                    
                ],
                colReorder: true,
                "bPaginate": false,
                "columnDefs": [
                    // { "width": "10%", "targets": 0 },
                    // { "width": "10%", "targets": 1 },
                    // { "width": "20%", "targets": 4 }, 
                    // { "width": "20%", "targets": 5 }, 
                    { "width": "20%", "targets": "datatable-photo" }, 
                    { "width": "20%", "targets": "datatable-video" }, 
                    { "width": "15%", "targets": "datatable-comments" } 
                    
                ] 
            })
            .buttons()
            .container()
            .appendTo("#datatable-buttons_wrapper .col-md-6:eq(0)");
});
