$(document).ready(function () {
    function user() {
        var tel = $('#reg-window-tel');
        var ok = $('#ok');
        var city = $('#city');
        var cst = $('#cost');
        var box = $('div.box-points');
        var itemTabindex = $('input.item-tabindex');
        var cost = 0;
        var dialogWindow = $('#dialogWindow');
        var dialogWindowMessage = $('#dialogWindow div.message-text');
        var data = {
            city: '',
            comment: '',
            points: []
        }
        ok.bind(
            'click',
            function (e) {
                dialogWindow
                    .addClass('dw-hide')
                    .removeClass('dw-show');
            })
        itemTabindex.bind("keyup",function (e) {
            $(this).removeClass('item-error');
        })
        this.costAdd = function (val) {
            cost = val;
        }
        this.costVal = function () {
            return cost;
        }
        this.dataClear = function () {
            data.points = [];
        }
        this.dataPointAdd = function (item) {
            data.points.push(item);
        }
        this.dataCityAdd = function (value) {
            data.city = value;
        }
        this.dataCommentAdd = function (value) {
            data.comment = value;
        }
        this.dataVal = function(){
            return data;
        }
        this.dataPointsLength = function () {
            return data.points.length;
        }
        this.dialogMessage = function (message) {
            dialogWindowMessage.html(message);
            dialogWindow.addClass('dw-show').removeClass('dw-hide');
        }
        this.setCityServer = function (city) {
            if (city == "DEFAULT") {
                this.dialogMessage("К сожалению нам не удалось определить ваш город, пожалуйста выберите его из списка");
            }
            else {
                var jsonText = JSON.stringify({ city: city });
                $.ajax({
                    type: "POST",
                    url: "/Ajax/cityServer",
                    data: jsonText,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data, textStatus) {
                        if (data.status == "OK") {
                            //user.setCity(data.city);
                        }
                        else {
                            if (data.status == "FAIL") {
                                user.setCity(" Ваш город");
                                user.dialogMessage("Что-то пошло не так, выберите город из списка");
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
                        user.setCity(resp.GeocoderMetaData.AddressDetails.Country.AddressLine);
                        user.setCityServer(resp.GeocoderMetaData.AddressDetails.Country.AddressLine);
                    }
                    else {
                        user.setCityServer('DEFAULT');
                    }
                });
            });
        }
        this.setCity = function (cityVal) {
            this.dataCityAdd(cityVal);
            city.html(cityVal);
        }
        this.setCost = function (costVal) {
            this.costAdd(costVal);
            cst.html(costVal);
            cst.parent().addClass('item-total-blue');
        }
        this.prepareForCost = function()
        {
            cst.html("XXX");
            cst.parent().removeClass('item-total-blue');
        }
        this.checkPoints = function (data)
        {
            var items = box.find("input.item-tabindex");
            var length = data.length;
            var fl = true;
            for (var i = 0; i < length; i++)
            {
                if(data[i])
                {
                    $(items[i]).addClass("item-error");
                    if (fl)
                    {
                        fl = false;
                        $(items[i]).focus();
                    }
                }
            }
            return fl;
        }
    }
    //$('#reg-window-tel').mask("(999) 999-99-99");
    var myMap = null;
    ymaps.ready(function (e) {
        myMap = new ymaps.Map('map', {
            center: [55.753994, 37.622093],
            zoom: 9
        });
    });
    var user = new user();
    var parthClone = null;
    var paramAutocomplet = {
        valueKey: 'name',
        titleKey: 'name',
        source: [{
            url: "/Ajax/AjaxRequestCity?cityName=%QUERY%",
            type: 'remote',
            getValue: function (item) {
                return (item['name'] + item['city'])
            },
            getTitle: function (item) {
                return (item['name'] + item['city'])
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

    $('a.item-move-up').bind('click', pushUp);
    $('a.item-move-down').bind('click', pushDown);
    $.fn.inputKeyUp = function()
    {
        if ($(this).hasClass('item-comment')) {
            user.dataCommentAdd($(this).val());
            return;
        }
        if ($(this).hasClass('item-point') || $(this).hasClass('item-num') || $(this).hasClass('item-porch')) {

            user.dataClear();
            var parent = null;
            if ($(this).hasClass('item-point')) {
                parent = $(this).parent().parent().parent();
            }
            else {
                parent = $(this).parent().parent();
            }
            var inputArray = parent.find('.item-tabindex');
            var legth = inputArray.length;
            for (var i = 0; i < legth; i++) {
                var street = $(inputArray[i]).val();
                var streetNum = $(inputArray[++i]).val();
                var houseNum = $(inputArray[++i]).val();
                if (street != "" && streetNum != "") {
                    user.dataPointAdd({ street: street, streetNum: streetNum, houseNum: houseNum });
                }
                else {
                    break;
                }
            }
            if (user.dataPointsLength() < 2) {
                user.dataClear();
            }
        }
    }
    $('input').keyup(function (e) {
        $(this).inputKeyUp();
    });
    $('a.item-remove').click(function (e) {
        var container = $(this).parent().parent();
        var length = container.children("div.item").length;
        if (!$(this).parent().prev().length) {
            $(this).parent().next().find('input.item-point').attr({ 'placeholder': 'Пункт отправления' });
        }
        if (length == 3) {
            $(this).parent().animate({ height: 'toggle' }, 200).remove();
            container.find('a.item-remove').each(function (index, element) { $(element).addClass('item-hidden'); });
        }
        else {
            $(this).parent().animate({ height: 'toggle' }, 200).remove();
        }
        tabindexUpdate(container.parent().find('.item-tabindex'));
        return false;
    });

    parthClone = $(document).find('div.item').first().clone(true);
    parthClone.find('input.item-point').attr({ 'placeholder': 'Пункт прибытия' });
    parthClone.find('a.item-hidden').removeClass('item-hidden');

    $('input.item-point').autocomplete(paramAutocomplet);

    $('a.item-add-point').click(function (e) {
        var prev = $(this).parent().prev();
        var div = prev.children("div.item");
        if (div.length < 10) {
            var divLength = div.length * 3 + 1;
            var divLast = $(div.last());
            var clone = parthClone.clone(true);
            clone.find('input.item-point').addClass('_noObject').attr({ 'tabindex': divLength });
            clone.find('input.item-num').attr({ 'tabindex': divLength + 1 });
            clone.find('input.item-porch').attr({ 'tabindex': divLength + 2 });
            clone.appendTo(prev);
            prev.find('input._noObject').removeClass('_noObject').autocomplete(paramAutocomplet);
        }
        if (div.length == 2) {
            prev.find('a.item-hidden').each(function (key, value) {
                $(value).removeClass('item-hidden');
            });
        }
        tabindexUpdate(prev.parent().find('.item-tabindex'));
        return false;
    });
    function pushDown() {
        var parent = $(this).parent().parent();
        if (parent.next().length) {
            parent.animate({ height: 'toggle' }, 200).insertAfter(parent.next()).animate({ height: 'toggle' }, 200);
            tabindexUpdate(parent.parent().parent().find('.item-tabindex'));
            if (!parent.prev().prev().length) {
                parent.find('input.item-point').attr({ 'placeholder': 'Пункт прибытия' });
                parent.prev().find('input.item-point').attr({ 'placeholder': 'Пункт отправления' });
            }
        }
        return false;
    };
    function pushUp() {
        var parent = $(this).parent().parent();
        if (parent.prev().length) {
            parent.animate({ height: 'toggle' }, 200).insertBefore(parent.prev()).animate({ height: 'toggle' }, 200);
            tabindexUpdate(parent.parent().parent().find('item-tabindex'));
            if (!parent.prev().length) {
                parent.find('input.item-point').attr({ 'placeholder': 'Пункт отправления' });
                parent.next().find('input.item-point').attr({ 'placeholder': 'Пункт прибытия' });
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
        user.setCityServer(city);
    })

    $('#rout').click(function (e) {
        if (user.dataPointsLength() > 1) {
            $('div.item-loader').removeClass('item-loader-hidden');
            user.prepareForCost();
            var jsonText = JSON.stringify(user.dataVal());
            $.ajax({
                type: "POST",
                url: "/Ajax/RoutPath",
                data: { list: jsonText },
                dataType: "json",
                success: function (data, textStatus) {
                    if (data.status) {
                        if (data.te) {
                            user.setCost(data.cost);
                        }
                        else {
                            user.checkPoints(data.data);
                            user.dialogMessage("Не все указанные улицы или номера домов найдены, проверьте наличие ошибок.");
                        }
                    }
                    $('div.item-loader').addClass('item-loader-hidden');
                    $('.item-price-cost').focus();
                }
            });
        }
        else {
            user.dialogMessage("Для расчета нужны минимум начальная и конечная точки.");
        }
        return false;
    });
    $('#order').click(function (e) {
        if (user.dataPointsLength() > 1) {
            $('div.item-loader').removeClass('item-loader-hidden');
            user.prepareForCost();
            var jsonText = JSON.stringify(user.dataVal());
            $.ajax({
                type: "POST",
                url: "/Ajax/OrderPath",
                data: { list: jsonText },
                dataType: "json",
                success: function (data, textStatus) {
                    if (data.status) {
                        if (data.te) {
                            user.setCost(data.cost);
                        }
                        else {
                            user.checkPoints(data.data);
                            user.dialogMessage("Не все указанные улицы или номера домов найдены, проверьте наличие ошибок.");
                        }
                    }
                    $('div.item-loader').addClass('item-loader-hidden');
                    $('.item-price-cost').focus();
                }
            });
        }
        else {
            user.dialogMessage("Для расчета нужны минимум начальная и конечная точки.");
        }
        return false;
    });


    $.fn.animateRotate = function (angle, duration, easing, complete) {
        var args = $.speed(duration, easing, complete);
        var step = args.step;
        return this.each(function (i, e) {
            args.complete = $.proxy(args.complete, e);
            args.step = function (now) {
                $.style(e, 'transform', 'rotate(' + now + 'deg)');
                if (step) return step.apply(e, arguments);
            };

            $({ deg: 0 }).animate({ deg: angle }, args);
        });
    };
    $("i.glyphicon-animate").click(function (e) {
        var _this = $(this);
        var parent = _this.parent().parent().parent();
        var menu = _this.parent().parent();
        var array = menu.children('div');
        var count = array.length;
        var parentFast = 100;
        var itemFast = 200;
        if (_this.hasClass('glyphicon-menu-hamburger'))
        {
            _this.removeClass('glyphicon-menu-hamburger').addClass('glyphicon-remove');
            _this.animateRotate(180);
            parent.addClass('menu-xs-container');
            array.each(function (count, item) {
                var c = (count+8) * 20;
                $(item).addClass('menu-xs-slider').css({ 'left': '-'+c+'px' });
            });
            parent.animate({ 'height': '100%' }, parentFast);
            array.each(function (count, item) {
                if (count != 6)
                {
                    $(array[count]).animate({ 'left': '0px' }, itemFast);
                }
                else
                {
                    $(array[count]).animate({ 'left': '0px' }, itemFast, function (e) {
                        array.each(function (count, item) {
                            $(item).css({ 'left': 'auto' });
                            parent.addClass('height-100').css({ 'height': 'auto' });
                        });
                    });
                }
            });
        }
        else
        {
            array.each(function (count, item) {
                var c = (count + 8) * 20;
                $(array[count]).animate({ 'left': '-' + c + 'px' }, itemFast, function (e) {
                    parent.css({ 'height': '100%' }).removeClass('height-100');
                    parent.animate({ 'height': '28px' }, parentFast, function (e) {
                        array.each(function (count, item) {
                            $(item).removeClass('menu-xs-slider').css({'left':'auto'});
                        });
                        parent.removeClass('menu-xs-container');
                        _this.removeClass('glyphicon-remove').addClass('glyphicon-menu-hamburger');
                        _this.animateRotate(180);
                    });
                });
            });
        }
    });
});