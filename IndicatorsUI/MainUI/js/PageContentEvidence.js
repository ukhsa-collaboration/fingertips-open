function goToContentPage() {
    lock();
    setPageMode(PAGE_MODES.CONTENT);
    showAndHidePageElements();
    unlock();
}
pages.add(PAGE_MODES.CONTENT, {
    id: 'content',
    title: 'Evidence',
    icon: 'evidence',
    goto: goToContentPage,
    gotoName: 'goToContentPage',
    needsContainer: false,
    jqIds: ['content']
});

