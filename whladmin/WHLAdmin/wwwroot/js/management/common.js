$(function () {

  $('.whl-action-refresh-page').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    window.location.reload();
  });

  $('.whl-action-view-audit').on('click', function (e) {
    e.preventDefault();
    const entityType = $(e.currentTarget).data('entitytype');
    const entityId = $(e.currentTarget).data('entityid');
    $.get(auditViewerUrl + '?entityType=' + entityType + '&entityId=' + entityId)
      .done(function (response) {
        $('#auditViewerModal').find('div.modal-body').html(response);
        $('#auditViewerModal').modal('show');
      });
  });

});