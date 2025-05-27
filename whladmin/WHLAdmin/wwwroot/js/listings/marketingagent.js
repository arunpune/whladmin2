$(function () {

  $('.whl-action-edit-marketingagent').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    const listingId = $(e.currentTarget).data('listingid');
    $.get(listingMarketingAgentEditUrl + '?listingId=' + listingId)
      .done(function (response) {
        $('#whl-title-marketingagent-action').html('Edit Marketing Agent');
        $('#hidMarketingAgentAction').val('EDIT');
        $('#marketingAgentEditorModal').find('div.modal-body').html(response);
        $('#marketingAgentEditorModal').modal('show');
        $('#lblMarketingAgentSaveErrorMessage').html('');
        $('#divMarketingAgentSaveErrorMessage').hide();
      });
  });

  $('.whl-action-save-marketingagent').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('#lblMarketingAgentSaveErrorMessage').html('');
    $('#divMarketingAgentSaveErrorMessage').hide();

    const a = $('#hidMarketingAgentAction').val();
    const url = listingMarketingAgentEditUrl;
    const data = {
      ListingId: $('#MarketingAgentListingId').val(),
      MarketingAgentInd: $('#MarketingAgentInd').prop('checked'),
      MarketingAgentId: $('#MarketingAgentId').val(),
      MarketingAgentApplicationLink: $('#MarketingAgentApplicationLink').val()
    };
    $.post(url, data)
      .done(function (response) {
        $('#marketingAgentEditorModal').find('div.modal-body').html('');
        $('#marketingAgentEditorModal').modal('hide');
        const listingId = $('#PageListingId').val();
        fnShowToast('SUCCESS', 'Marketing agent for Listing #' + listingId + ' updated, refreshing page.');
      })
      .fail(function (e) {
        const message = e.responseJSON && e.responseJSON.message && e.responseJSON.message.length > 0 ? e.responseJSON.message : 'Failed to save marketing agent';
        $('#lblMarketingAgentSaveErrorMessage').html(e.responseJSON.message);
        $('#divMarketingAgentSaveErrorMessage').show();
      });
  });

  $('.whl-action-view-marketingagent').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    const agentId = $(e.currentTarget).data('agentid');
    $.get(listingMarketinAgentViewUrl + '?agentId=' + agentId)
      .done(function (response) {
        $('#marketingAgentViewerModal').find('div.modal-body').html(response);
        $('#marketingAgentViewerModal').modal('show');
      });
  });

});