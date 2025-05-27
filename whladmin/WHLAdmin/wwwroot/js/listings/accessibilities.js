$(function () {

  $('.whl-action-edit-accessibilities').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    const listingId = $(e.currentTarget).data('listingid');
    $.get(listingAccessibilitiesEditUrl + '?listingId=' + listingId)
      .done(function (response) {
        $('#whl-title-accessibility-action').html('Edit Accessibilities');
        $('#hidAccessibilityAction').val('EDIT');
        $('#accessibilityEditorModal').find('div.modal-body').html(response);
        $('#accessibilityEditorModal').modal('show');
        $('#lblAccessibilitySaveErrorMessage').html('');
        $('#divAccessibilitySaveErrorMessage').hide();
      });
  });

  $('.whl-action-save-accessibility').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('#lblAccessibilitySaveErrorMessage').html('');
    $('#divAccessibilitySaveErrorMessage').hide();

    let selectedAccessibilityCds = '';
    $("input:checkbox[name=selAccessibilities]:checked").each(function () {
      selectedAccessibilityCds += (selectedAccessibilityCds.length > 0 ? ',' : '') + $(this).val();
    });

    const a = $('#hidAccessibilityAction').val();
    const url = a === 'EDIT' ? listingAccessibilitiesEditUrl : listingAccessibilitiesAddUrl;
    const data = {
      ListingId: $('#AccessibilityListingId').val(),
      AccessibilityCds: selectedAccessibilityCds
    };
    $.post(url, data)
      .done(function (response) {
        $('#accessibilityEditorModal').find('div.modal-body').html('');
        $('#accessibilityEditorModal').modal('hide');
        const listingId = $('#PageListingId').val();
        fnShowToast('SUCCESS', 'Accessibility for Listing #' + listingId + ' updated, refreshing page.');
      })
      .fail(function (e) {
        const message = e.responseJSON && e.responseJSON.message && e.responseJSON.message.length > 0 ? e.responseJSON.message : 'Failed to save accessibilities';
        $('#lblAccessibilitySaveErrorMessage').html(e.responseJSON.message);
        $('#divAccessibilitySaveErrorMessage').show();
      });
  });

});