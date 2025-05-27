$(function () {

  $('.whl-action-edit-fundingsources').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    const listingId = $(e.currentTarget).data('listingid');
    $.get(listingFundingSourcesEditUrl + '?listingId=' + listingId)
      .done(function (response) {
        $('#whl-title-fundingsource-action').html(fundingSourcesCount > 0 ? 'Edit Funding Sources' : 'Add Funding Sources');
        $('#hidFundingSourceAction').val('EDIT');
        $('#fundingSourceEditorModal').find('div.modal-body').html(response);
        $('#fundingSourceEditorModal').modal('show');
        $('#lblFundingSourceSaveErrorMessage').html('');
        $('#divFundingSourceSaveErrorMessage').hide();
      });
  });

  $('.whl-action-save-fundingsource').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('#lblFundingSourceSaveErrorMessage').html('');
    $('#divFundingSourceSaveErrorMessage').hide();

    let selectedFundingSourceIds = '';
    $("input:checkbox[name=selFundingSources]:checked").each(function () {
      selectedFundingSourceIds += (selectedFundingSourceIds.length > 0 ? ',' : '') + $(this).val();
    });

    const a = $('#hidFundingSourceAction').val();
    const url = a === 'EDIT' ? listingFundingSourcesEditUrl : listingFundingSourcesAddUrl;
    const data = {
      ListingId: $('#FundingSourceListingId').val(),
      FundingSourceIds: selectedFundingSourceIds
    };
    $.post(url, data)
      .done(function (response) {
        $('#fundingSourceEditorModal').find('div.modal-body').html('');
        $('#fundingSourceEditorModal').modal('hide');
        const listingId = $('#PageListingId').val();
        fnShowToast('SUCCESS', 'Funding Sources for Listing #' + listingId + ' updated, refreshing page.');
      })
      .fail(function (e) {
        const message = e.responseJSON && e.responseJSON.message && e.responseJSON.message.length > 0 ? e.responseJSON.message : 'Failed to save amenities';
        $('#lblFundingSourceSaveErrorMessage').html(e.responseJSON.message);
        $('#divFundingSourceSaveErrorMessage').show();
      });
  });

});