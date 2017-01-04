'use strict';



'use strict';

function goToEnglandPage() {
    lock();
    setPageMode(PAGE_MODES.ENGLAND);
    showAndHidePageElements();
    var imgUrl = FT.url.img;
    var englandhtml = templates.render('england', { images: imgUrl });

    pages.getContainerJq().html(englandhtml);
    
    unlock();
}


templates.add('england', '<div class="englandImage"><img src="{{images}}/englandTemp/EnglandOverview.jpg" alt="England" /></div>');

pages.add(PAGE_MODES.ENGLAND, {
    id: 'england',
    title: 'England',
    icon: 'england',
    goto: goToEnglandPage,
    gotoName: 'goToEnglandPage',
    needsContainer: true,
    jqIds: ['england']
});
