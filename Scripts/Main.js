var Sortable = {
    baseUrl: '',
    sortBy: 0,
    searchTerm: '',
    Search() {
        var searchKey = $('#textSearch').val();
        window.location.href = Sortable.baseUrl + "?id=" + searchKey;
        //window.location.href = Sortable.baseUrl + searchKey;
    },

    Sort(sortBy) {
        var isDesc = true;

        const urlParams = new URLSearchParams(window.location.search);

        var isDescOriginal = urlParams.get('isDesc');
        const sortByOriginal = urlParams.get('sortBy');

        if (sortByOriginal == sortBy) {
            if (isDescOriginal == 'true') {
                isDesc = false;
            }
        }
        window.location.href = Sortable.baseUrl + "?sortBy=" + sortBy + "&isDesc=" + isDesc;
    }
};

var apiHandler = {
    GET(url) {
        $.ajax({
            url: url,
            type: 'GET',
            success: function (res) {
                debugger;
            }
        });
    },
    POST(url, object) {
        object = {
            Id: 5,
            Name: "asd",
            // ....
        }
        $.ajax({
            url: url,
            type: 'GET',
            data: object,
            success: function (res) {
                debugger;
            }
        });
    },
    DELETE(url) {
        if (confirm("Are you sure you want to delete?")) {
            $.ajax({
                url: url,
                type: 'GET',
                success: function (res) {
                    if (res.Success == true) {
                        debugger;
                        location.href = res.returnUrl;

                    } else {
                        alert(res.Message);
                    }
                }
            });
        } else {
            alert("Delete cancelled");
        }

    }
};