(function () {
    initTwitterFeed();
})();

function initTwitterFeed() {
    ajaxMonitor.setCalls(1);    
    getTweets();    
    ajaxMonitor.monitor(displayPage);
}

function displayPage() {
    var html = templates.render('tweets', {tweets : tweets});    
    $('#twitter_feed').append(html);   
} 

function getTweets() {
    $.ajax({
            type: 'GET',
            url: '/tweets/' + Twitter_Handle,
            data: {},
            cache: false,
            contentType: 'application/json',
            dataType: 'jsonp',
            success: getTweetsCallback,
            error: function() {}
    });
}

function getTweetsCallback(obj) {
    tweets = obj;
    
    ajaxMonitor.callCompleted();
}

templates.add('tweets','{{#tweets}}<div class="tweet">{{{Text}}}<div class="time">{{CreatedDate}}</div></div>{{/tweets}}');

