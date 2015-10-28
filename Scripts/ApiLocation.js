$(document).ready(function () {
    function user() {
        this.cost
        this.city = $('#city');
        this.dialogWindow = $('#dialogWindow');
        this.dialogWindowMessage = $('#dialogWindow div.dw-show-message');
        this.data = {
            city:'',
            comment:'',
            points:[]
        }
        this.dialogMessage = function (message, typeButtons) {
            var html = message + '<br />';
            switch (typeButtons) {
                case 1: this.dialogWindowMessage.html(html + '<button class="btn btn-default" id="ok">Продолжить</button>'); $('#ok').bind('click', function (e) { user.dialogWindow.addClass('dw-hide').removeClass('dw-show'); }); break;
                default: this.dialogWindowMessage.html(html + '<button class="btn btn-default" id="ok">Продолжить</button>'); $('#ok').bind('click', function (e) { user.dialogWindow.addClass('dw-hide').removeClass('dw-show'); }); break;
            }
            user.dialogWindow.addClass('dw-show').removeClass('dw-hide');
        }
        this.setCityServer = function(city)
        {
            if (city == "DEFAULT")
            {
                this.dialogMessage("К сожалению нам не удалось определить ваш город, пожалуйста выберите его из списка",2);
            }
            else
            {
                var jsonText = JSON.stringify({ city: city });
                $.ajax({
                    type: "POST",
                    url: "/Ajax/cityServer",
                    data: jsonText,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data, textStatus) {
                        if (data.status == "OK") {
                            user.setCity(data.city);
                        }
                        else {
                            if(data.status == "DEFAULT")
                            {
                                user.dialogMessage("Ваш город не найден или не обслуживается, приносим свои извинения!(Попробуйте выбрать город из списка)", 1);
                            }
                            else
                            {
                                if (data.status == "FAIL") {
                                    user.dialogMessage("Что-то пошло не так, выберите город из списка", 1);
                                }
                            }
                        }
                    }
                });
            }
        }
        this.geolocation = function () {
            ymaps.ready(function () {
                
                ymaps.geolocation.get({
                    // Выставляем опцию для определения положения по ip
                    provider: 'yandex',
                    // Автоматически геокодируем полученный результат.
                    autoReverseGeocode: true
                }).then(function (result) {

                    var geo = result.geoObjects.get(0);
                    if (geo != null) {
                        var resp = geo.properties.get('metaDataProperty');
                        user.setCityServer(resp.GeocoderMetaData.AddressDetails.Country.AddressLine);
                    }
                    else {
                        user.setCityServer('DEFAULT');
                    }
                });
            });
        }
        this.setCity = function(city)
        {
            this.data.city = city;
            this.city.html('<i class="glyphicon glyphicon-home"></i> ' + city);
        }
        this.setCost = function(cost)
        {
            this.cost = cost;
            $('#cost').html(cost);
        }
    }
    /*var myMap = new ymaps.Map('map', {
        center: [55.753994, 37.622093],
        zoom: 9
    });*/
    var user = new user();
    var parthClone = null;
    var paramAutocomplet = {
        valueKey: 'name',
        titleKey: 'name',
        source: [{
            url: "/Ajax/AjaxRequestCity?cityName=%QUERY%",
            type: 'remote',
            getValue: function (item) {
                return (item['name'] + item['city']).replace(/^(ул |пер )/, '')
            },
            getTitle: function (item) {
                return item['name'] + item['city']//.replace(/^(ул |пер )/, '')
            },
            ajax: {
                dataType: 'json'
            }
        }]
    };
    $(window).load(function (e) {
        $(window).resize();
        user.geolocation();
    })

    $('a.push-up').bind('click', pushUp);
    $('a.push-down').bind('click', pushDown);
    $('input').keyup(function (e) {
        if ($(this).hasClass('text-comment')) {
            user.data.comment = $(this).val();
            return;
        }
        var parent = null;
        user.data.points = [];
        if ($(this).hasClass('text-info')) {
            parent = $(this).parent().parent().parent();
        }
        else {
            if ($(this).hasClass('text-num')) {
                parent = $(this).parent().parent();
            }
        }
        var inputArray = parent.find('input.tabindex');
        var legth = inputArray.length;
        for (var i = 0; i < legth; i++) {
            var street = $(inputArray[i]).val();
            var streetNum = $(inputArray[++i]).val();
            if (street != "" && streetNum != "") {
                user.data.points.push({ street: street, streetNum: streetNum });
            }
            else {
                break;
            }
        }
        if (user.data.points.length < 2) {
            user.data.points = [];
        }

    });
    $('a.push-remove').click(function (e) {
        var container = $(this).parent().parent();
        var length = container.children("div").length;
        if (!$(this).parent().prev().length)
        {
            $(this).parent().next().find('input.text-info').attr({ 'placeholder': 'Пункт отправления' });
        }
        if (length == 3) {
            $(this).parent().animate({ height: 'toggle' }, 200).remove();
            container.find('a.push-remove').each(function (index, element) { $(element).addClass('hidden'); });
        }
        else
        {
            $(this).parent().animate({ height: 'toggle' }, 200).remove();
        }
    });

    parthClone = $(document).find('div.col-xs-12.mrg').first().clone(true);
    parthClone.find('input.text-info').attr({ 'placeholder': 'Пункт прибытия' });
    parthClone.find('a.hidden').removeClass('hidden');

    $('input.text-info').autocomplete(paramAutocomplet);
    
    $('a.push-add').click(function (e) {
        var prev = $(this).parent().parent().prev()
        var div = prev.children("div");
        if (div.length < 10) {
            var divLength = div.length * 2 + 1;
            var divLast = $(div.last());
            var clone = parthClone.clone(true);
            clone.find('input.text-info').addClass('_noObject').attr({ 'tabindex': divLength });
            clone.find('input.text-num').attr({ 'tabindex': divLength });
            clone.appendTo(prev);
            prev.find('input._noObject').removeClass('_noObject').autocomplete(paramAutocomplet);
        }
        if (div.length == 2)
        {
            prev.find('a.hidden').each(function (key, value) {
                $(value).removeClass('hidden');
            });
        }
        return false;
    });
    function pushDown() {
        var parent = $(this).parent().parent();
        if (parent.next().length) {
            parent.animate({ height: 'toggle' }, 200).insertAfter(parent.next()).animate({ height: 'toggle' }, 200);
            tabindexUpdate(parent.parent().parent().find('input.tabindex'));
            if (!parent.prev().prev().length) {
                parent.find('input.text-info').attr({ 'placeholder': 'Пункт прибытия' });
                parent.prev().find('input.text-info').attr({ 'placeholder': 'Пункт отправления' });
            }
        }
        return false;
    };
    function pushUp() {
        var parent = $(this).parent().parent();
        if (parent.prev().length) {
            parent.animate({ height: 'toggle' }, 200).insertBefore(parent.prev()).animate({ height: 'toggle' }, 200);
            tabindexUpdate(parent.parent().parent().find('input.tabindex'));
            if(!parent.prev().length)
            {
                parent.find('input.text-info').attr({ 'placeholder': 'Пункт отправления' });
                parent.next().find('input.text-info').attr({ 'placeholder': 'Пункт прибытия' });
            }
        }
        return false;
    };
    function tabindexUpdate(inputArray) {
        var length = inputArray.length;
        for (var i = 0; i < length; i++) {
            $(inputArray[i]).attr({ 'tabindex': i + 1 });
        }
    }
    $('ul li a.city').click(function (e) {
        var city = $(this).html();
        $('#c').html('<i class="glyphicon glyphicon-home"></i> ' + city);
        user.setCity(city);
    })
    
    $('#rout').click(function (e) {
        if (user.data.points.length>1) {
            var jsonText = JSON.stringify(user.data);
            $.ajax({
                type: "POST",
                url: "/Ajax/RoutPath",
                data: { list: jsonText },
                dataType: "json",
                success: function (data, textStatus) {
                    if (data.cost !==  undefined) {
                        //забацать функцию setCost
                        user.setCost(data.cost);
                    }
                }
            });
        }
        else {
            user.dialogMessage("Для расчета нужны минимум начальная и конечная точки.", 1);
        }
    });
    $('#menu').click(function () {
        if ($("div.slider").css('width') == "0px")
        {
            $("div.slider").removeClass("slider").addClass("slider-on");//("slider", "slider-on", 500, "easeInOutQuad");
        }
        else {
            $("div.slider-on").removeClass("slider-on").addClass("slider");
        }
        
    });
});