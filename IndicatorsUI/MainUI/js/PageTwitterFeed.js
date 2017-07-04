(function () {
    displayTwitterFeed();
})();

function displayTwitterFeed() {
    $.ajax({
            type: 'GET',
            url: '/tweets/' + Twitter_Handle,
            data: {},
            cache: false,
            contentType: 'application/json',
            dataType: 'jsonp',
            success: function (tweets) {

                templates.add('tweets',
                    '{{#tweets}}<div class="tweet">{{{Text}}}<div class="time">{{CreatedDate}}</div></div>{{/tweets}}');

                var html = templates.render('tweets', { tweets: tweets });
                $('#twitter_feed').append(html);
            },
            error: function() {}
    });
}

